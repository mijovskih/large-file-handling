using LargeFileHandling.Exceptions;
using LargeFileHandling.Interfaces;
using LargeFileHandling.Models;

namespace LargeFileHandling.Services
{
    public sealed class FileTransferService : IFileTransferService
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

        public TransferReport Transfer(TransferRequest request)
        {
            ArgumentNullException.ThrowIfNull(request);

            using ISourceReader source = _sourceReaderFactory.Create(request.SourceFilePath);
            long length = source.Length;

            using IChunkReceiver receiver = _chunkReceiverFactory.Create(request.DestinationFilePath, length);

            var checksums = new List<ChunkChecksum>();
            int index = 0;
            
            for (long offset = 0; offset < length; offset += request.ChunkSize)
            {
                int chunkLength = (int)Math.Min(request.ChunkSize, length - offset);
                FileChunk chunk = source.Read(offset, chunkLength);

                string sourceHash = _hashCalculator.ComputeHash(chunk.Data);
                string destinationHash = receiver.Receive(chunk);

                if (sourceHash != destinationHash)
                    throw new ChunkVerificationException(index, offset, sourceHash, destinationHash);
            
                checksums.Add(new ChunkChecksum(index, offset, sourceHash));
                index++;
            }

            return new TransferReport(checksums);
        }
    }
}