#nullable enable

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Shadop.Archmage.Sdk
{
    /// <summary>
    /// File system abstraction for Archmage configuration loading.
    /// </summary>
    /// <remarks>
    /// <para>Implementations read local data only. Downloading remote content is the caller's
    /// responsibility and must be done before loading.</para>
    /// <para>When a file does not exist, <see cref="ReadAllBytes"/> and <see cref="ReadAllBytesAsync"/>
    /// throw <see cref="System.IO.FileNotFoundException"/>. The loader relies on this to skip missing
    /// override files.</para>
    /// <para>During asynchronous loading, any method may be called from a thread pool thread.</para>
    /// </remarks>
    public interface IFS
    {
        /// <summary>
        /// Reads all bytes from the specified file.
        /// </summary>
        /// <param name="path">The file path.</param>
        /// <returns>A byte array containing the contents of the file.</returns>
        /// <exception cref="System.IO.FileNotFoundException">The file does not exist.</exception>
        byte[] ReadAllBytes(string path);

        /// <summary>
        /// Asynchronously reads all bytes from the specified file.
        /// </summary>
        /// <param name="path">The file path.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A task that represents the asynchronous read operation, wrapping the file contents as a byte array.</returns>
        /// <exception cref="System.IO.FileNotFoundException">The file does not exist.</exception>
        Task<byte[]> ReadAllBytesAsync(string path, CancellationToken cancellationToken = default);

        /// <summary>
        /// Determines whether the specified file exists.
        /// </summary>
        /// <remarks>
        /// May return true for a missing file when an exact check is expensive; reading that file then
        /// throws <see cref="System.IO.FileNotFoundException"/>. Must not return false for an existing file.
        /// </remarks>
        /// <param name="path">The file to check.</param>
        /// <returns>false if the file is known not to exist; otherwise, true.</returns>
        bool FileExists(string path);

        /// <summary>
        /// Determines whether the given path refers to an existing directory.
        /// </summary>
        /// <remarks>
        /// Used to validate override roots before loading. May return true when the underlying storage has
        /// no directory concept or an exact check is expensive. Must not return false for an existing directory.
        /// </remarks>
        /// <param name="path">The path to test.</param>
        /// <returns>false if the directory is known not to exist; otherwise, true.</returns>
        bool DirectoryExists(string path);

        /// <summary>
        /// Prepares for the <see cref="FileExists"/> calls of an asynchronous load.
        /// </summary>
        /// <remarks>
        /// <see cref="Archmage.LoadAtlasAsync"/> calls this once per load, before any item is loaded, with every
        /// override file path it will pass to <see cref="FileExists"/> on this instance. It is not called by
        /// <see cref="Archmage.LoadAtlas"/> or when there are no override files. Implementations that check file
        /// existence cheaply can return a completed task.
        /// </remarks>
        /// <param name="paths">The override file paths, as they will be passed to <see cref="FileExists"/>.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task PrepareAsync(IReadOnlyCollection<string> paths, CancellationToken cancellationToken = default);
    }
}
