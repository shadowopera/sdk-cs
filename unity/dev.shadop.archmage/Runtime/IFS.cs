using System.Threading;
using System.Threading.Tasks;

namespace Shadop.Archmage
{
    /// <summary>
    /// File system abstraction for Archmage configuration loading.
    /// </summary>
    public interface IFS
    {
        /// <summary>
        /// Reads all bytes from the specified file.
        /// </summary>
        /// <param name="path">The file path.</param>
        /// <returns>A byte array containing the contents of the file.</returns>
        byte[] ReadAllBytes(string path);

        /// <summary>
        /// Asynchronously reads all bytes from the specified file.
        /// </summary>
        /// <param name="path">The file path.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A task that represents the asynchronous read operation, wrapping the file contents as a byte array.</returns>
        Task<byte[]> ReadAllBytesAsync(string path, CancellationToken cancellationToken = default);

        /// <summary>
        /// Determines whether the specified file exists.
        /// </summary>
        /// <param name="path">The file to check.</param>
        /// <returns>true if the caller has the required permissions and path contains the name of an existing file; otherwise, false.</returns>
        bool FileExists(string path);

        /// <summary>
        /// Determines whether the given path refers to an existing directory on disk.
        /// </summary>
        /// <param name="path">The path to test.</param>
        /// <returns>true if path refers to an existing directory; false if the directory does not exist or an error occurs when trying to determine if the specified directory exists.</returns>
        bool DirectoryExists(string path);
    }
}
