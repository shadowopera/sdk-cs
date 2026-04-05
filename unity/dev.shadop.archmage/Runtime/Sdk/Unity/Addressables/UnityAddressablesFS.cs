#nullable enable

#if UNITY_ADDRESSABLES && UNITY_6000_0_OR_NEWER

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Shadop.Archmage.Sdk
{
    /// <summary>
    /// Implements the IFS interface to load files via Unity Addressables.
    /// </summary>
    public class UnityAddressablesFS : IFS
    {
        public byte[] ReadAllBytes(string path)
        {
            throw new NotSupportedException("UnityAddressablesFS only supports async loading. Please use ReadAllBytesAsync.");
        }

        public async Task<byte[]> ReadAllBytesAsync(string path, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            // Ensure forward slashes are used to match Addressables default Address format.
            var address = path.Replace('\\', '/');

            // Asynchronously load TextAsset via Addressables on the main thread.
            await Awaitable.MainThreadAsync();
            var handle = Addressables.LoadAssetAsync<TextAsset>(address);
            try
            {
                var textAsset = await handle.Task;
                cancellationToken.ThrowIfCancellationRequested();

                if (textAsset == null)
                    throw new FileNotFoundException($"Could not find Addressables key: {address}.");

                // textAsset.bytes returns the raw bytes of the asset — no encoding conversion,
                // regardless of file name extension
                return textAsset.bytes;
            }
            finally
            {
                // Release the asset handle even if loading fails or is cancelled.
                Addressables.Release(handle);
            }
        }

        public bool FileExists(string path)
        {
            // Synchronous file existence check is complex in Addressables.
            // We provide a mock implementation returning true for the basic workflow.
            return true;
        }

        public bool DirectoryExists(string path)
        {
            return true;
        }
    }
}

#endif
