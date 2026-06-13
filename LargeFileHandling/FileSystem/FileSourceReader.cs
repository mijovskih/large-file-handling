using LargeFileHandling.Interfaces;
using LargeFileHandling.Models;
using Microsoft.Win32.SafeHandles;

namespace LargeFileHandling.FileSystem
{
    public sealed class FileSourceReader : ISourceReader
    {
        private readonly SafeFileHandle _handle;
        public long Length { get; }

        public FileSourceReader(string path)
        {
            _handle = File.OpenHandle(path, FileMode.Open, FileAccess.Read, FileShare.Read, FileOptions.Asynchronous);
            Length = RandomAccess.GetLength(_handle);
        }

        public async Task<FileChunk> ReadAsync(long offset, int length, CancellationToken cancellationToken)
        {
            var buffer = new byte[length]; // Allocating a buffer to the length of a chunk.
            int total = 0; // Total bytes read so far.

            while (total < length)
            {
                int read = await RandomAccess.ReadAsync(_handle, buffer.AsMemory(total), offset + total, cancellationToken); // Read into the buffer; increase the offset by the total bytes read so far.
                
                if (read == 0) // If read equals 0, then we reached the end of file before reading the allocated chunk size.
                    throw new EndOfStreamException($"Expected {length} bytes at offset {offset}, but reached end of file after reading {total} bytes.");
                
                total += read;
            }

            return new FileChunk(offset, buffer); // At this point the whole chunk has been read.
        }

        public void Dispose() => _handle.Dispose();
    }
}