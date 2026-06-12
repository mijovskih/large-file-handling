using LargeFileHandling.Exceptions;
using LargeFileHandling.Interfaces;
using LargeFileHandling.Models;

namespace LargeFileHandling.Services
{
    public class FileTransferService : IFileTransferService
    {
        private readonly ISourceReaderFactory _sourceReaderFactory;
        private readonly IChunkReceiverFactory _chunkReceiverFactory;
        private readonly IHashCalculator _hashCalculator;

        public FileTransferService(ISourceReaderFactory sourceReaderFactory, IChunkReceiverFactory chunkReceiverFactory, IHashCalculator hashCalculator)
        {
            ArgumentNullException.ThrowIfNull(sourceReaderFactory);
            ArgumentNullException.ThrowIfNull(chunkReceiverFactory);
            ArgumentNullException.ThrowIfNull(hashCalculator);

            _sourceReaderFactory = sourceReaderFactory;
            _chunkReceiverFactory = chunkReceiverFactory;
            _hashCalculator = hashCalculator;
        }

        public void Transfer(TransferRequest request)
        {
            ArgumentNullException.ThrowIfNull(request);

            using ISourceReader source = _sourceReaderFactory.Create(request.SourceFilePath);
            long length = source.Length;

            using IChunkReceiver receiver = _chunkReceiverFactory.Create(request.DestinationFilePath, length);

            for (long offset = 0; offset < length; offset += request.ChunkSize)
            {
                int chunkLength = (int)Math.Min(request.ChunkSize, length - offset);
                FileChunk chunk = source.Read(offset, chunkLength);

                string sourceHash = _hashCalculator.ComputeHash(chunk.Data);
                string destinationHash = receiver.Receive(chunk);
                
                if (sourceHash != destinationHash)
                    throw new ChunkVerificationException(offset, sourceHash, destinationHash);
            }
        }
    }
}