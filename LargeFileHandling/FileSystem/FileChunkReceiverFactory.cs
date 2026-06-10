using LargeFileHandling.Interfaces;

namespace LargeFileHandling.FileSystem
{
    public sealed class FileChunkReceiverFactory : IChunkReceiverFactory
    {
        public IChunkReceiver Create(string path, long totalLength) => new FileChunkReceiver(path, totalLength);
    }
}