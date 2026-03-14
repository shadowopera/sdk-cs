#nullable enable

#if UNITY_5_3_OR_NEWER

using System.IO;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Shadop.Archmage
{
    /// <summary>
    /// Implements the IFS interface to load files via Unity Resources.
    /// Paths are resolved relative to any Resources folder; file extensions are stripped automatically.
    /// Synchronous loading must be called from the main thread.
    /// </summary>
    public class UnityResourcesFS : IFS
    {
        public byte[] ReadAllBytes(string path)
        {
            var resourcePath = StripExtension(path);
            var textAsset = Resources.Load<TextAsset>(resourcePath);

            if (textAsset == null)
                throw new FileNotFoundException($"Could not find Resources asset: {resourcePath}.");

            var bytes = textAsset.bytes;
            Resources.UnloadAsset(textAsset);
            return bytes;
        }

#if UNITY_6000_0_OR_NEWER
        public async Task<byte[]> ReadAllBytesAsync(string path, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var resourcePath = StripExtension(path);

            await Awaitable.MainThreadAsync();

            var resourceRequest = Resources.LoadAsync<TextAsset>(resourcePath);
            await resourceRequest;
            cancellationToken.ThrowIfCancellationRequested();

            var textAsset = resourceRequest.asset as TextAsset;

            if (textAsset == null)
                throw new FileNotFoundException($"Could not find Resources asset: {resourcePath}.");

            var bytes = textAsset.bytes;
            Resources.UnloadAsset(textAsset);
            return bytes;
        }
#else
        public Task<byte[]> ReadAllBytesAsync(string path, CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException("UnityResourcesFS async loading requires Unity 6 or newer.");
        }
#endif

        public bool FileExists(string path)
        {
            return true;
        }

        public bool DirectoryExists(string path)
        {
            return true;
        }

        private static string StripExtension(string path)
        {
            return Path.ChangeExtension(path, null).Replace('\\', '/');
        }
    }
}

#endif
