#nullable enable

#if UNITY_ADDRESSABLES && UNITY_6000_0_OR_NEWER

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Shadop.Archmage.Sdk
{
    /// <summary>
    /// Implements the IFS interface to load files via Unity Addressables.
    /// Only asynchronous loading is supported.
    /// </summary>
    /// <remarks>
    /// Content in remote groups must be downloaded before loading.
    /// </remarks>
    public class UnityAddressablesFS : IFS
    {
        public byte[] ReadAllBytes(string path)
        {
            throw new NotSupportedException("UnityAddressablesFS only supports async loading. Please use ReadAllBytesAsync.");
        }

        public async Task<byte[]> ReadAllBytesAsync(string path, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var address = ToAddress(path);

            // Resolve the location first: a missing key yields an empty list instead of
            // an InvalidKeyException logged by LoadAssetAsync.
            await Awaitable.MainThreadAsync();
            var locationsHandle = Addressables.LoadResourceLocationsAsync(address, typeof(TextAsset));
            try
            {
                var locations = await locationsHandle.Task;
                cancellationToken.ThrowIfCancellationRequested();

                if (locationsHandle.Status != AsyncOperationStatus.Succeeded)
                    throw new IOException($"Failed to resolve Addressables key: {address}.", locationsHandle.OperationException);
                if (locations.Count == 0)
                    throw new FileNotFoundException($"Could not find Addressables key: {address}.", path);

                var handle = Addressables.LoadAssetAsync<TextAsset>(locations[0]);
                try
                {
                    var textAsset = await handle.Task;
                    cancellationToken.ThrowIfCancellationRequested();

                    if (handle.Status != AsyncOperationStatus.Succeeded || textAsset is null)
                        throw new IOException($"Failed to load Addressables asset: {address}.", handle.OperationException);

                    // textAsset.bytes returns the raw bytes of the asset — no encoding conversion,
                    // regardless of file name extension
                    return textAsset.bytes;
                }
                finally
                {
                    // Release the asset handle even if loading fails or is canceled.
                    Addressables.Release(handle);
                }
            }
            finally
            {
                Addressables.Release(locationsHandle);
            }
        }

        /// <summary>
        /// Always returns true. Catalog lookups must run on the main thread, while the loader may call
        /// this method from a worker thread, so a missing file is reported by the read instead.
        /// </summary>
        public bool FileExists(string path)
        {
            return true;
        }

        /// <summary>
        /// Always returns true. Addressables has no directory concept.
        /// </summary>
        public bool DirectoryExists(string path)
        {
            return true;
        }

        private static string ToAddress(string path)
        {
            // Ensure forward slashes are used to match Addressables default Address format.
            return path.Replace('\\', '/');
        }
    }
}

#endif
