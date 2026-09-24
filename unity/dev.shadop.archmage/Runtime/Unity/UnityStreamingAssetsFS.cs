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
    /// Only asynchronous loading is supported.
    /// </summary>
    public class UnityStreamingAssetsFS : IFS
    {
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

            await Awaitable.MainThreadAsync();
            using var request = UnityWebRequest.Get(uri);
            cancellationToken.ThrowIfCancellationRequested();

            // Wire cancellation to abort the in-flight request.
            // The registration is disposed asynchronously to avoid blocking the main thread
            // and holding a reference to the request after it has been released.
            await using var registration = cancellationToken.Register(request.Abort);
            await request.SendWebRequest();

            cancellationToken.ThrowIfCancellationRequested();

            if (request.result != UnityWebRequest.Result.Success)
            {
                var msg = $"Failed to load StreamingAssets file: {uri}. Error: {request.error}";
                if (IsNotFound(request, fullPath))
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

        private static bool IsNotFound(UnityWebRequest request, string fullPath)
        {
            if (!IsUri(fullPath))
                return !File.Exists(fullPath);
            if (request.responseCode == 404)
                return true;
            // Files inside the APK are local, so a failed read means the entry is missing.
            return fullPath.StartsWith("jar:", StringComparison.Ordinal);
        }
    }
}

#endif
