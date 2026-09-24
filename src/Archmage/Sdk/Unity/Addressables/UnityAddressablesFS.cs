#nullable enable

#if UNITY_ADDRESSABLES && UNITY_6000_0_OR_NEWER

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;

namespace Shadop.Archmage.Sdk
{
    /// <summary>
    /// Implements the IFS interface to load files via Unity Addressables.
    /// Only asynchronous loading is supported.
    /// </summary>
    /// <remarks>
    /// Content in remote groups must be downloaded before loading. An instance serves a single load;
    /// create a new instance for each load.
    /// </remarks>
    public class UnityAddressablesFS : IFS
    {
        bool _prepared;
        HashSet<string>? _missing;

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
        /// Returns false for paths that <see cref="PrepareAsync"/> found missing; otherwise, true.
        /// </summary>
        public bool FileExists(string path)
        {
            var missing = _missing;
            return missing is null || !missing.Contains(ToAddress(path));
        }

        /// <summary>
        /// Always returns true. Addressables has no directory concept.
        /// </summary>
        public bool DirectoryExists(string path)
        {
            return true;
        }

        /// <summary>
        /// Resolves all paths on the main thread in one pass and records the missing ones for
        /// <see cref="FileExists"/>, which may run on a worker thread.
        /// </summary>
        /// <exception cref="InvalidOperationException">Called more than once on this instance.</exception>
        public async Task PrepareAsync(IReadOnlyCollection<string> paths, CancellationToken cancellationToken = default)
        {
            if (_prepared)
                throw new InvalidOperationException("UnityAddressablesFS serves a single load. Create a new instance for each load.");
            _prepared = true;

            cancellationToken.ThrowIfCancellationRequested();

            await Awaitable.MainThreadAsync();
            var handles = new List<(string Address, AsyncOperationHandle<IList<IResourceLocation>> Handle)>(paths.Count);
            try
            {
                foreach (var path in paths)
                {
                    var address = ToAddress(path);
                    handles.Add((address, Addressables.LoadResourceLocationsAsync(address, typeof(TextAsset))));
                }

                var missing = new HashSet<string>();
                foreach (var (address, handle) in handles)
                {
                    if (!handle.IsDone)
                        await handle.Task;
                    cancellationToken.ThrowIfCancellationRequested();

                    if (handle.Status != AsyncOperationStatus.Succeeded)
                        throw new IOException($"Failed to resolve Addressables key: {address}.", handle.OperationException);
                    if (handle.Result.Count == 0)
                        missing.Add(address);
                }

                _missing = missing;
            }
            finally
            {
                foreach (var (_, handle) in handles)
                    Addressables.Release(handle);
            }
        }

        private static string ToAddress(string path)
        {
            // Ensure forward slashes are used to match Addressables default Address format.
            return path.Replace('\\', '/');
        }
    }
}

#endif
