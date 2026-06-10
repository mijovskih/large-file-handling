using LargeFileHandling.Interfaces;
using LargeFileHandling.Models;

namespace LargeFileHandling.Services
{
    public class FileTransferService : IFileTransferService
    {
        private readonly ISourceReaderFactory _sourceReaderFactory;
        private readonly IChunkReceiverFactory _chunkReceiverFactory;

        public FileTransferService(ISourceReaderFactory sourceReaderFactory, IChunkReceiverFactory chunkReceiverFactory)
        {
            ArgumentNullException.ThrowIfNull(sourceReaderFactory);
            ArgumentNullException.ThrowIfNull(chunkReceiverFactory);

            _sourceReaderFactory = sourceReaderFactory;
            _chunkReceiverFactory = chunkReceiverFactory;
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
                receiver.Receive(chunk);
            }
        }
    }
}