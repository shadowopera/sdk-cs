#nullable enable

using System.Threading;
using System.Threading.Tasks;

namespace Shadop.Archmage.Sdk
{
    /// <summary>
    /// File system abstraction for Archmage configuration loading.
    /// </summary>
    /// <remarks>
    /// <para>When a file does not exist, <see cref="ReadAllBytes"/> and <see cref="ReadAllBytesAsync"/>
    /// must throw <see cref="System.IO.FileNotFoundException"/>. <see cref="Archmage.LoadAtlas"/> and
    /// <see cref="Archmage.LoadAtlasAsync"/> rely on this to skip missing override files.</para>
    /// <para>If <see cref="MainThreadOnly"/> is true for the main IFS or for any override IFS, loading must start on
    /// the main thread. LoadAtlas and LoadAtlasAsync then call the methods of every IFS on the calling thread.</para>
    /// <para>Otherwise, if MainThreadOnly is false for every IFS, LoadAtlas and LoadAtlasAsync read the files of atlas
    /// items with <see cref="ReadAllBytes"/>, and may call the methods on thread pool threads.</para>
    /// </remarks>
    public interface IFS
    {
        /// <summary>
        /// Gets whether the methods of this file system can be called only on the main thread.
        /// </summary>
        /// <remarks>
        /// Return true if the methods work only on the main thread, such as those that call Unity APIs. Return
        /// false if they can be called on any thread.
        /// </remarks>
        bool MainThreadOnly { get; }

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
        /// If checking is expensive, an implementation may skip the check and return true.
        /// </remarks>
        /// <param name="path">The file to check.</param>
        /// <returns>true if the file exists or the check was skipped; otherwise, false.</returns>
        bool FileExists(string path);

        /// <summary>
        /// Determines whether the specified directory exists.
        /// </summary>
        /// <remarks>
        /// If the storage has no directories, or checking is expensive, an implementation may skip the check
        /// and return true.
        /// </remarks>
        /// <param name="path">The directory to check.</param>
        /// <returns>true if the directory exists or the check was skipped; otherwise, false.</returns>
        bool DirectoryExists(string path);
    }
}
