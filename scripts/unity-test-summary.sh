#!/usr/bin/env bash

# Prints a readable summary of ArchmageDev PlayMode test runs.
#
# Usage:
#   scripts/unity-test-summary.sh report <mode> <results.xml> <log>
#       summarizes one run: test results, failure messages and AddressablesCostTests measurements
#   scripts/unity-test-summary.sh compare <assetdb-results.xml> <packed-results.xml>
#       compares test results of an Asset Database run and a packed run side by side

[[ "$TRACE" ]] && set -x

function xpath() {
    xmllint --xpath "$2" "$1" 2> /dev/null || true
}

function modeName() {
    case "$1" in
        assetdb) echo "Asset Database (Use Asset Database)" ;;
        packed) echo "Packed (Use Existing Build)" ;;
        *) echo "$1" ;;
    esac
}

# testResults <results.xml>: prints "fullname<TAB>result<TAB>duration" per test case
function testResults() {
    local n i
    n="$(xpath "$1" 'count(//test-case)')"
    for ((i = 1; i <= ${n:-0}; i++)); do
        printf '%s\t%s\t%s\n' \
            "$(xpath "$1" "string((//test-case)[$i]/@fullname)")" \
            "$(xpath "$1" "string((//test-case)[$i]/@result)")" \
            "$(xpath "$1" "string((//test-case)[$i]/@duration)")"
    done
}

function report() {
    local mode="$1" results="$2" log="$3"

    echo "Mode:  $(modeName "$mode")"
    echo "Tests: total=$(xpath "$results" 'string(/test-run/@total)')" \
        "passed=$(xpath "$results" 'string(/test-run/@passed)')" \
        "failed=$(xpath "$results" 'string(/test-run/@failed)')" \
        "skipped=$(xpath "$results" 'string(/test-run/@skipped)')"
    echo

    echo "Test results (duration is the wall time of each test, not a cost measurement)"
    {
        printf 'TEST\tRESULT\tDURATION\n'
        testResults "$results" | awk -F'\t' '{ printf "%s\t%s\t%.2fs\n", $1, $2, $3 }'
    } | column -t -s $'\t' | sed 's/^/  /'

    local failed name
    failed="$(testResults "$results" | awk -F'\t' '$2 == "Failed" { print $1 }')"
    if [[ -n "$failed" ]]; then
        echo
        echo "Failures"
        while IFS= read -r name; do
            echo "  - $name"
            xpath "$results" "string(//test-case[@fullname=\"$name\"]/failure/message)" | sed 's/^/      /'
        done <<< "$failed"
    fi

    echo
    echo "Addressables load cost (from AddressablesCostTests, frames stretched to ~16 ms)"
    if ! grep -q '^\[AddressablesCost\] mode=' "$log" 2> /dev/null; then
        echo "  not measured (run with --filter AddressablesCostTests)"
        return
    fi
    # [AddressablesCost] mode=assetdb loadDelay=0.02 concurrent=False missingRoots=1 frames=57 ms=980.1 reads(main=1, worker=13)
    grep '^\[AddressablesCost\] mode=' "$log" | awk '
        BEGIN { print "SETUP\tLOADING\tMISSING OVERRIDE ROOTS\tFRAMES\tTIME\tREADS (MAIN/WORKER)" }
        {
            setup = ""; loading = ""; roots = ""; frames = ""; ms = ""; mainReads = ""; workerReads = ""
            for (i = 2; i <= NF; i++) {
                split($i, kv, "=")
                gsub(/[(),]/, "", kv[2])
                if (kv[1] == "mode") setup = kv[2]
                else if (kv[1] == "loadDelay") setup = setup " delay=" kv[2] "s"
                else if (kv[1] == "concurrent") loading = (kv[2] == "True") ? "concurrent" : "sequential"
                else if (kv[1] == "missingRoots") roots = kv[2]
                else if (kv[1] == "frames") frames = kv[2]
                else if (kv[1] == "ms") ms = kv[2]
                else if (kv[1] == "reads(main") mainReads = kv[2]
                else if (kv[1] == "worker") workerReads = kv[2]
            }
            printf "%s\t%s\t%s\t%s\t%.0f ms\t%s/%s\n", setup, loading, roots, frames, ms, mainReads, workerReads
        }' | column -t -s $'\t' | sed 's/^/  /'
}

function compare() {
    local assetdb="$1" packed="$2"

    echo "Test results by mode"
    {
        printf 'TEST\tASSET DATABASE\tPACKED\n'
        join -t $'\t' -a 1 -a 2 -e '-' -o 0,1.2,2.2 \
            <(testResults "$assetdb" | cut -f1,2 | sort) \
            <(testResults "$packed" | cut -f1,2 | sort)
    } | column -t -s $'\t' | sed 's/^/  /'
}

case "$1" in
    report)
        [[ $# -eq 4 ]] || { >&2 echo "Usage: $0 report <mode> <results.xml> <log>"; exit 1; }
        report "$2" "$3" "$4"
        ;;
    compare)
        [[ $# -eq 3 ]] || { >&2 echo "Usage: $0 compare <assetdb-results.xml> <packed-results.xml>"; exit 1; }
        compare "$2" "$3"
        ;;
    *)
        >&2 echo "Usage: $0 report <mode> <results.xml> <log> | compare <assetdb-results.xml> <packed-results.xml>"
        exit 1
        ;;
esac
