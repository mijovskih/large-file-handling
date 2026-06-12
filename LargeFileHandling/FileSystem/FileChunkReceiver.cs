using LargeFileHandling.Interfaces;
using LargeFileHandling.Models;
using Microsoft.Win32.SafeHandles;

namespace LargeFileHandling.FileSystem
{
    public sealed class FileChunkReceiver : IChunkReceiver
    {
        private readonly SafeFileHandle _handle;
        private readonly IHashCalculator _hashCalculator;

        public FileChunkReceiver(string path, long totalLength, IHashCalculator hashCalculator)
        {
            ArgumentNullException.ThrowIfNull(hashCalculator);

            _handle = File.OpenHandle(path, FileMode.Create, FileAccess.ReadWrite, FileShare.None);
            RandomAccess.SetLength(_handle, totalLength);
            _hashCalculator = hashCalculator;
        }

        public string Receive(FileChunk chunk)
        {
            ArgumentNullException.ThrowIfNull(chunk);
            RandomAccess.Write(_handle, chunk.Data, chunk.Offset);

            // After writing the chunk, we read it back from disk to calculate the hash.
            // It's important to read it back from disk and not in-memory, in case the write operation got corrupted.
            byte[] writteBytes = ReadBack(chunk.Offset, chunk.Data.Length);
            return _hashCalculator.ComputeHash(writteBytes);
        }

        private byte[] ReadBack(long offset, int length)
        {
            var buffer = new byte[length];
            int total = 0;

            while (total < length)
            {
                int read = RandomAccess.Read(_handle, buffer.AsSpan(total), offset + total);
                if (read == 0)
                    throw new EndOfStreamException($"Expected to read back {length} bytes at offset {offset}, but reached end of file after reading {total} bytes.");
                total += read;
            }
            
            return buffer;
        }

        public void Dispose() => _handle.Dispose();
    }
}