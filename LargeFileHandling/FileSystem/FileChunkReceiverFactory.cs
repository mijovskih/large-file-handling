using LargeFileHandling.Interfaces;

namespace LargeFileHandling.FileSystem
{
    public sealed class FileChunkReceiverFactory : IChunkReceiverFactory
    {
        private readonly IHashCalculator _hashCalculator;
        
        public FileChunkReceiverFactory(IHashCalculator hashCalculator)
        {
            ArgumentNullException.ThrowIfNull(hashCalculator);
            _hashCalculator = hashCalculator;
        }

        public IChunkReceiver Create(string path, long totalLength) => new FileChunkReceiver(path, totalLength, _hashCalculator);
    }
}