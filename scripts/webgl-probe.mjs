// Serves a WebGL build and runs one probe case in headless Chrome.
// Usage: node scripts/webgl-probe.mjs <build dir> <case> [timeout seconds]
// Prints the page's [Probe] console lines. Exits 0 on "[Probe] DONE", 2 on timeout.

import { spawn } from 'node:child_process';
import { createReadStream, statSync } from 'node:fs';
import { createServer } from 'node:http';
import { extname, join, normalize } from 'node:path';

const [buildDir, probeCase, timeoutArg] = process.argv.slice(2);
const timeoutMs = (Number(timeoutArg) || 60) * 1000;
const chromePath = process.env.CHROME || '/Applications/Google Chrome.app/Contents/MacOS/Google Chrome';

const types = {
    '.html': 'text/html', '.js': 'application/javascript', '.wasm': 'application/wasm',
    '.json': 'application/json', '.data': 'application/octet-stream', '.css': 'text/css',
    '.png': 'image/png', '.ico': 'image/x-icon', '.bundle': 'application/octet-stream',
    '.hash': 'text/plain',
};

// Unity names compressed files *.br / *.gz; serve them with Content-Encoding.
const server = createServer((req, res) => {
    const path = normalize(join(buildDir, decodeURIComponent(new URL(req.url, 'http://x').pathname)));
    let stat;
    try {
        stat = statSync(path);
    } catch {
        res.writeHead(404).end();
        return;
    }
    if (stat.isDirectory()) {
        res.writeHead(404).end();
        return;
    }
    const headers = { 'Cache-Control': 'no-store' };
    let name = path;
    if (name.endsWith('.br')) {
        headers['Content-Encoding'] = 'br';
        name = name.slice(0, -3);
    } else if (name.endsWith('.gz')) {
        headers['Content-Encoding'] = 'gzip';
        name = name.slice(0, -3);
    }
    headers['Content-Type'] = types[extname(name)] || 'application/octet-stream';
    res.writeHead(200, headers);
    createReadStream(path).pipe(res);
});
await new Promise(r => server.listen(0, '127.0.0.1', r));
const pageUrl = `http://127.0.0.1:${server.address().port}/index.html?case=${probeCase}`;

const chrome = spawn(chromePath, [
    '--headless=new', '--remote-debugging-port=0', '--no-first-run', '--no-default-browser-check',
    `--user-data-dir=${process.env.TMPDIR || '/tmp'}/webgl-probe-chrome-${process.pid}`,
    '--enable-unsafe-swiftshader', 'about:blank',
], { stdio: ['ignore', 'ignore', 'pipe'] });

const wsUrl = await new Promise((resolve, reject) => {
    let buf = '';
    chrome.stderr.on('data', d => {
        buf += d;
        const m = buf.match(/DevTools listening on (ws:\/\/\S+)/);
        if (m) resolve(m[1]);
    });
    chrome.on('exit', () => reject(new Error('Chrome exited early:\n' + buf)));
});

let exitCode = 2;
const finish = () => {
    chrome.kill();
    server.close();
    process.exit(exitCode);
};

const ws = new WebSocket(wsUrl);
let nextId = 1;
const pending = new Map();
const send = (method, params = {}, sessionId) => new Promise(resolve => {
    const id = nextId++;
    pending.set(id, resolve);
    ws.send(JSON.stringify({ id, method, params, sessionId }));
});

ws.onmessage = ev => {
    const msg = JSON.parse(ev.data);
    if (msg.id && pending.has(msg.id)) {
        pending.get(msg.id)(msg.result);
        pending.delete(msg.id);
        return;
    }
    if (msg.method === 'Runtime.consoleAPICalled') {
        const text = msg.params.args.map(a => a.value ?? a.description ?? '').join(' ').trimEnd();
        if (text.includes('[Probe]') || msg.params.type === 'error')
            console.log(text);
        if (/\[Probe\].* DONE$/.test(text.trim())) {
            exitCode = 0;
            finish();
        }
    } else if (msg.method === 'Runtime.exceptionThrown') {
        console.log('[page exception]', msg.params.exceptionDetails.exception?.description ?? msg.params.exceptionDetails.text);
    }
};

ws.onopen = async () => {
    const { targetId } = await send('Target.createTarget', { url: 'about:blank' });
    const { sessionId } = await send('Target.attachToTarget', { targetId, flatten: true });
    await send('Runtime.enable', {}, sessionId);
    await send('Page.navigate', { url: pageUrl }, sessionId);
    setTimeout(() => {
        console.log(`[harness] timeout after ${timeoutMs / 1000}s`);
        finish();
    }, timeoutMs);
};
