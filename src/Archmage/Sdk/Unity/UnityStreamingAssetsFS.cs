#nullable enable

#if UNITY_6000_0_OR_NEWER

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Shadop.Archmage.Sdk
{
    /// <summary>
    /// Implements the IFS interface to load files from Unity StreamingAssets via UnityWebRequest.
    /// Paths are resolved relative to Application.streamingAssetsPath.
    /// Only asynchronous loading is supported, and it must be started from the main thread.
    /// </summary>
    /// <remarks>
    /// On WebGL, StreamingAssets is deployed to the web server along with the build, so files are downloaded
    /// over HTTP. A 404 response is treated as a missing file. Any other error, such as a 403 response or a
    /// network failure, makes the load fail.
    /// </remarks>
    public class UnityStreamingAssetsFS : IFS
    {
        public bool MainThreadOnly => true;

        public byte[] ReadAllBytes(string path)
        {
            throw new NotSupportedException("UnityStreamingAssetsFS only supports async loading. Please use ReadAllBytesAsync.");
        }

        public async Task<byte[]> ReadAllBytesAsync(string path, CancellationToken cancellationToken = default)
        {
            var fullPath = ResolvePath(path);

            // On Android, streamingAssetsPath is already a jar:file:// URI.
            // On other platforms it is a plain file system path and requires the file:// scheme.
            var uri = IsUri(fullPath) ? fullPath : "file://" + fullPath;

            using var request = UnityWebRequest.Get(uri);
            cancellationToken.ThrowIfCancellationRequested();

            // Wire cancellation to abort the in-flight request.
            // The registration is disposed asynchronously to avoid blocking the main thread
            // and holding a reference to the request after it has been released.
            // Cancel() runs the callback on the thread that calls it, but Unity allows Abort only on the main thread,
            // so the callback posts Abort there. A post that runs after the request finishes does nothing.
            var mainThread = SynchronizationContext.Current;
            var finished = false;
            await using var registration = cancellationToken.Register(() =>
                mainThread.Post(_ => { if (!finished) request.Abort(); }, null));
            try
            {
                await request.SendWebRequest();
            }
            finally
            {
                finished = true;
            }

            cancellationToken.ThrowIfCancellationRequested();

            if (request.result != UnityWebRequest.Result.Success)
            {
                var msg = $"Failed to load StreamingAssets file: {uri}. Error: {request.error}";
                if (IsFileNotFound(request, fullPath))
                    throw new FileNotFoundException(msg, path);
                throw new IOException(msg);
            }

            return request.downloadHandler.data;
        }

        /// <summary>
        /// Checks the file system directly where StreamingAssets is a plain directory.
        /// Returns true on platforms where it is a URI (Android, WebGL).
        /// </summary>
        public bool FileExists(string path)
        {
            var fullPath = ResolvePath(path);
            return IsUri(fullPath) || File.Exists(fullPath);
        }

        /// <summary>
        /// Checks the file system directly where StreamingAssets is a plain directory.
        /// Returns true on platforms where it is a URI (Android, WebGL).
        /// </summary>
        public bool DirectoryExists(string path)
        {
            var fullPath = ResolvePath(path);
            return IsUri(fullPath) || Directory.Exists(fullPath);
        }

        private static string ResolvePath(string path)
        {
            // Combine with StreamingAssets root, normalizing to forward slashes.
            return Application.streamingAssetsPath + "/" + path.Replace('\\', '/');
        }

        private static bool IsUri(string fullPath)
        {
            return fullPath.Contains("://");
        }

        private static bool IsFileNotFound(UnityWebRequest request, string fullPath)
        {
            if (!IsUri(fullPath))
                return !File.Exists(fullPath);
            if (request.responseCode == 404)
                return true;
            // Unity returns 404 for a missing entry inside the APK in tests, but does not document it.
            // Files inside the APK are local, so treat any failed read as a missing entry.
            return fullPath.StartsWith("jar:", StringComparison.Ordinal);
        }
    }
}

#endif
