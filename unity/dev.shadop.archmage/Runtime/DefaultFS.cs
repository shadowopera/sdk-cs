#nullable enable

using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Shadop.Archmage
{
    /// <summary>
    /// Default implementation of the IFS interface using standard System.IO operations.
    /// </summary>
    public class DefaultFS : IFS
    {
        /// <inheritdoc />
        public byte[] ReadAllBytes(string path)
        {
            return File.ReadAllBytes(path);
        }

        /// <inheritdoc />
        public Task<byte[]> ReadAllBytesAsync(string path, CancellationToken cancellationToken = default)
        {
            return File.ReadAllBytesAsync(path, cancellationToken);
        }

        /// <inheritdoc />
        public bool FileExists(string path)
        {
            return File.Exists(path);
        }

        /// <inheritdoc />
        public bool DirectoryExists(string path)
        {
            return Directory.Exists(path);
        }
    }
}
