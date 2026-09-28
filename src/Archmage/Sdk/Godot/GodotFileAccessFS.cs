#nullable enable

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Godot;
using FileAccess = Godot.FileAccess;

namespace Shadop.Archmage.Sdk
{
    /// <summary>
    /// Implements the IFS interface to read files with Godot's <c>FileAccess</c>. Paths can be <c>res://</c> paths,
    /// <c>user://</c> paths or operating system paths, and files in mounted resource packs can be read too.
    /// </summary>
    /// <remarks>
    /// The methods can be called on any thread. Call <c>ProjectSettings.LoadResourcePack</c> before loading starts:
    /// mounting a resource pack while files are being read is not supported.
    /// </remarks>
    public class GodotFileAccessFS : IFS
    {
        /// <inheritdoc />
        public bool MainThreadOnly => false;

        /// <inheritdoc />
        public byte[] ReadAllBytes(string path)
        {
            path = NormalizePath(path);
            var data = FileAccess.GetFileAsBytes(path);
            var error = FileAccess.GetOpenError();
            if (error == Error.Ok)
                return data;

            // Why not check for ERR_FILE_NOT_FOUND? On Android, a missing file in the APK gives ERR_CANT_OPEN.
            if (!FileAccess.FileExists(path))
                throw new FileNotFoundException($"Could not find file: {path}.", path);
            throw new IOException($"Failed to read file: {path}. Error: {error}");
        }

        /// <inheritdoc />
        /// <remarks>
        /// Godot has no asynchronous file read, so this method calls <see cref="ReadAllBytes"/> on a thread pool
        /// thread.
        /// </remarks>
        public Task<byte[]> ReadAllBytesAsync(string path, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Task.Run(() => ReadAllBytes(path), cancellationToken);
        }

        /// <inheritdoc />
        public bool FileExists(string path)
        {
            return FileAccess.FileExists(NormalizePath(path));
        }

        /// <inheritdoc />
        /// <remarks>
        /// Always returns true for <c>res://</c> paths, because <c>DirAccess</c> does not report them reliably after
        /// the project is exported or a resource pack is mounted.
        /// </remarks>
        public bool DirectoryExists(string path)
        {
            path = NormalizePath(path);
            if (path.StartsWith("res://", StringComparison.Ordinal))
                return true;
            return DirAccess.DirExistsAbsolute(path);
        }

        // The loader joins paths with Path.Combine, which uses '\' on Windows.
        private static string NormalizePath(string path)
        {
            return path.Replace('\\', '/');
        }
    }
}
