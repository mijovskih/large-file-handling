using LargeFileHandling.Interfaces;
using LargeFileHandling.Models;
using Microsoft.Win32.SafeHandles;

namespace LargeFileHandling.FileSystem
{
    public sealed class FileChunkReceiver : IChunkReceiver
    {
        private readonly SafeFileHandle _handle;

        public FileChunkReceiver(string path, long totalLength)
        {
            _handle = File.OpenHandle(path, FileMode.Create, FileAccess.Write, FileShare.None);
            RandomAccess.SetLength(_handle, totalLength);
        }

        public void Receive(FileChunk chunk)
        {
            ArgumentNullException.ThrowIfNull(chunk);
            RandomAccess.Write(_handle, chunk.Data, chunk.Offset);
        }

        public void Dispose() => _handle.Dispose();
    }
}