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
using UnityEngine.ResourceManagement.ResourceProviders;

namespace Shadop.Archmage.Sdk
{
    /// <summary>
    /// Implements the <see cref="IFS"/> interface to load files via Unity Addressables, reading all files in each
    /// encountered asset bundle in one go.
    /// Only <c>LoadAtlasAsync</c> is supported, and it must be called from the main thread.
    /// </summary>
    /// <remarks>
    /// <para>On WebGL, <see cref="UnityAddressablesFS"/> takes about one frame per file, because Unity completes
    /// at most one asynchronous asset load per frame. This class instead reads every file in an asset bundle
    /// synchronously the first time a file in that bundle is requested, and keeps the other files in memory until
    /// they are requested.</para>
    /// <para>This class reads every file in the bundle, so a bundle that also holds other files costs extra time
    /// and memory. Keep configs in asset bundles of their own. Files that are never requested stay in memory
    /// until the instance is garbage collected.</para>
    /// </remarks>
    public class UnityAddressablesGreedyFS : IFS
    {
        // Reads files that cannot be taken from _cache.
        private readonly UnityAddressablesFS _fallback = new UnityAddressablesFS();

        // Asset path -> bytes of files read from bundles and not requested yet.
        // Case-insensitive only as a safeguard.
        private readonly Dictionary<string, byte[]> _cache = new Dictionary<string, byte[]>(StringComparer.OrdinalIgnoreCase);

        // Bundle internal ID -> the task that caches the bundle's files; its result is false when GetAssetBundle()
        // returns null. Entries are never removed, so the files of each bundle are cached only once.
        // Why keep an entry whose task failed? A bundle that fails to load fails the whole load, so retrying is
        // pointless.
        private readonly Dictionary<string, Task<bool>> _cacheTasks = new Dictionary<string, Task<bool>>();

        public bool MainThreadOnly => true;

        public byte[] ReadAllBytes(string path)
        {
            throw new NotSupportedException("UnityAddressablesGreedyFS only supports async loading. Please use ReadAllBytesAsync.");
        }

        public async Task<byte[]> ReadAllBytesAsync(string path, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var address = ToAddress(path);

            var locationsHandle = Addressables.LoadResourceLocationsAsync(address, typeof(TextAsset));
            try
            {
                // Even if the handle is already completed, awaiting its Task still resumes on the next frame,
                // so skip the await.
                if (!locationsHandle.IsDone)
                    await locationsHandle.Task;
                cancellationToken.ThrowIfCancellationRequested();
                var locations = locationsHandle.Result;

                if (locationsHandle.Status != AsyncOperationStatus.Succeeded)
                    throw new IOException($"Failed to resolve Addressables key: {address}.", locationsHandle.OperationException);
                if (locations.Count == 0)
                    throw new FileNotFoundException($"Addressables key not found: {address}.", path);

                var location = locations[0];
                var bundleLocation = GetBundleLocation(location);
                // The file has no bundle, e.g. in the Editor's "Use Asset Database" play mode.
                if (bundleLocation is null)
                    return await _fallback.ReadAllBytesAsync(path, cancellationToken);

                var assetPath = Addressables.ResourceManager.TransformInternalId(location);
                if (TakeCached(assetPath, out var bytes))
                    return bytes;

                // All reads of files in a bundle share one task, including reads that start while it is running.
                var bundleKey = bundleLocation.InternalId;
                if (!_cacheTasks.TryGetValue(bundleKey, out var task))
                {
                    task = CacheBundleFilesAsync(bundleLocation);
                    _cacheTasks[bundleKey] = task;
                }

                // If the bundle fails to load, every read of a file in it fails with the same exception.
                var bundleCached = await task;
                cancellationToken.ThrowIfCancellationRequested();

                if (bundleCached && TakeCached(assetPath, out bytes))
                    return bytes;

                // GetAssetBundle() returned null, or an earlier read already took this file from the cache.
                return await _fallback.ReadAllBytesAsync(path, cancellationToken);
            }
            finally
            {
                Addressables.Release(locationsHandle);
            }
        }

        /// <summary>
        /// Always returns true. For a missing file, ReadAllBytesAsync throws FileNotFoundException.
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

        private bool TakeCached(string assetPath, out byte[] bytes)
        {
            // Drop the entry to free the memory. A later read of the same file goes to _fallback.
            return _cache.Remove(assetPath, out bytes!);
        }

        // BundledAssetProvider reads an asset from the first asset bundle among the location's dependencies.
        // A TextAsset depends on no other bundle, so the first dependency is its bundle.
        private static IResourceLocation? GetBundleLocation(IResourceLocation location)
        {
            if (!location.HasDependencies)
                return null;
            var dependency = location.Dependencies[0];
            if (!typeof(IAssetBundleResource).IsAssignableFrom(dependency.ResourceType))
                return null;
            return dependency;
        }

        // Copies the bytes of every TextAsset in the bundle into _cache. Returns false if GetAssetBundle() returns null.
        private async Task<bool> CacheBundleFilesAsync(IResourceLocation bundleLocation)
        {
            var rm = Addressables.ResourceManager;
            var bundleHandle = rm.ProvideResource<IAssetBundleResource>(bundleLocation);
            try
            {
                if (!bundleHandle.IsDone)
                    await bundleHandle.Task;

                if (bundleHandle.Status != AsyncOperationStatus.Succeeded)
                    throw new IOException($"Failed to load asset bundle: {bundleLocation.InternalId}.", bundleHandle.OperationException);

                var bundle = bundleHandle.Result?.GetAssetBundle();
                if (bundle == null)
                    return false;

                // Load synchronously: on WebGL, Unity completes at most one asynchronous asset load per frame.
                foreach (var name in bundle.GetAllAssetNames())
                {
                    var textAsset = bundle.LoadAsset<TextAsset>(name);
                    if (textAsset != null)
                        _cache[name] = textAsset.bytes;
                }
                return true;
            }
            finally
            {
                // Release the bundle as soon as its files are in _cache.
                bundleHandle.Release();
            }
        }

        private static string ToAddress(string path)
        {
            // The loader builds paths with Path.Combine, which uses backslashes on Windows. Default addresses are
            // asset paths, which use forward slashes.
            return path.Replace('\\', '/');
        }
    }
}

#endif
