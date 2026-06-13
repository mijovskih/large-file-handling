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
        private readonly IFileHasher _fileHasher;
        private readonly IProgressReporter _progressReporter;
        
        public FileTransferService(ISourceReaderFactory sourceReaderFactory, 
            IChunkReceiverFactory chunkReceiverFactory, 
            IHashCalculator hashCalculator,
            IFileHasher fileHasher,
            IProgressReporter progressReporter)
        {
            ArgumentNullException.ThrowIfNull(sourceReaderFactory);
            ArgumentNullException.ThrowIfNull(chunkReceiverFactory);
            ArgumentNullException.ThrowIfNull(hashCalculator);
            ArgumentNullException.ThrowIfNull(fileHasher);
            ArgumentNullException.ThrowIfNull(progressReporter);

            _sourceReaderFactory = sourceReaderFactory;
            _chunkReceiverFactory = chunkReceiverFactory;
            _hashCalculator = hashCalculator;
            _fileHasher = fileHasher;
            _progressReporter = progressReporter;
        }

        public async Task<TransferReport> TransferAsync(TransferRequest request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);
            
            var checksums = new List<ChunkChecksum>();

            using (ISourceReader source = _sourceReaderFactory.Create(request.SourceFilePath))
            using (IChunkReceiver receiver = _chunkReceiverFactory.Create(request.DestinationFilePath, source.Length))
            {
                long length = source.Length;
                long bytesTransferred = 0;
                int index = 0;
                
                for (long offset = 0; offset < length; offset += request.ChunkSize)
                {
                    int chunkLength = (int)Math.Min(request.ChunkSize, length - offset);
                    FileChunk chunk = await source.ReadAsync(offset, chunkLength, cancellationToken);

                    string sourceHash = _hashCalculator.ComputeHash(chunk.Data);
                    string destinationHash = await receiver.ReceiveAsync(chunk, cancellationToken);

                    if (sourceHash != destinationHash)
                        throw new ChunkVerificationException(index, offset, sourceHash, destinationHash);
                
                    checksums.Add(new ChunkChecksum(index, offset, sourceHash));
                    bytesTransferred += chunkLength;
                    _progressReporter.ShowProgress(bytesTransferred, length);
                    index++;
                }
            }

            string sourceFileHash = await _fileHasher.ComputeHashAsync(request.SourceFilePath, cancellationToken);
            string destinationFileHash = await _fileHasher.ComputeHashAsync(request.DestinationFilePath, cancellationToken);

            return new TransferReport(checksums, sourceFileHash, destinationFileHash);
        }
    }
}