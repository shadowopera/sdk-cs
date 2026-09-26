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
    /// Only asynchronous loading is supported, and it must be started from the main thread.
    /// </summary>
    /// <remarks>
    /// <para>Content in remote groups must be downloaded before loading.</para>
    /// <para>Each asset is released as soon as it has been read, so an asset bundle may be unloaded and
    /// loaded again during a single load. Optionally, holding a handle to an asset in the bundle until
    /// loading finishes keeps it loaded. An empty placeholder file in the bundle works well for this:</para>
    /// <code>
    /// var pin = Addressables.LoadAssetAsync&lt;TextAsset&gt;("Assets/Configs/placeholder.txt");
    /// await pin.Task;
    /// try
    /// {
    ///     await Archmage.LoadAtlasAsync(atlasFile, cfgRoot, atlas, options);
    /// }
    /// finally
    /// {
    ///     Addressables.Release(pin);
    /// }
    /// </code>
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
            var locationsHandle = Addressables.LoadResourceLocationsAsync(address, typeof(TextAsset));
            try
            {
                // Awaiting Task on a completed handle still resumes a frame later, so skip it.
                if (!locationsHandle.IsDone)
                    await locationsHandle.Task;
                cancellationToken.ThrowIfCancellationRequested();
                var locations = locationsHandle.Result;

                if (locationsHandle.Status != AsyncOperationStatus.Succeeded)
                    throw new IOException($"Failed to resolve Addressables key: {address}.", locationsHandle.OperationException);
                if (locations.Count == 0)
                    throw new FileNotFoundException($"Could not find Addressables key: {address}.", path);

                var handle = Addressables.LoadAssetAsync<TextAsset>(locations[0]);
                try
                {
                    if (!handle.IsDone)
                        await handle.Task;
                    cancellationToken.ThrowIfCancellationRequested();
                    var textAsset = handle.Result;

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
        /// Always returns true. A missing file is reported by the read instead.
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
