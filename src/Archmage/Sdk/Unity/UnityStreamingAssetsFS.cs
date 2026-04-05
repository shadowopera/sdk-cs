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
            // Combine with StreamingAssets root, normalizing to forward slashes.
            var normalizedPath = path.Replace('\\', '/');
            var fullPath = Application.streamingAssetsPath + "/" + normalizedPath;

            // On Android, streamingAssetsPath is already a jar:file:// URI.
            // On other platforms it is a plain file system path and requires the file:// scheme.
            var uri = fullPath.Contains("://") ? fullPath : "file://" + fullPath;

            await Awaitable.MainThreadAsync();

            using var request = UnityWebRequest.Get(uri);

            // Wire cancellation to abort the in-flight request.
            // The registration is disposed with the using block to avoid holding
            // a reference to the request after it has been released.
            using var registration = cancellationToken.Register(() => request.Abort());
            cancellationToken.ThrowIfCancellationRequested();

            await request.SendWebRequest();

            cancellationToken.ThrowIfCancellationRequested();

            if (request.result != UnityWebRequest.Result.Success)
                throw new IOException($"Failed to load StreamingAssets file: {uri}. Error: {request.error}");

            return request.downloadHandler.data;
        }

        public bool FileExists(string path)
        {
            return true;
        }

        public bool DirectoryExists(string path)
        {
            return true;
        }
    }
}

#endif
