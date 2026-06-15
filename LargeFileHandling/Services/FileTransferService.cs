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
        private readonly IRetryPolicy _retryPolicy;
        
        public FileTransferService(ISourceReaderFactory sourceReaderFactory, 
            IChunkReceiverFactory chunkReceiverFactory, 
            IHashCalculator hashCalculator,
            IFileHasher fileHasher,
            IProgressReporter progressReporter,
            IRetryPolicy retryPolicy)
        {
            ArgumentNullException.ThrowIfNull(sourceReaderFactory);
            ArgumentNullException.ThrowIfNull(chunkReceiverFactory);
            ArgumentNullException.ThrowIfNull(hashCalculator);
            ArgumentNullException.ThrowIfNull(fileHasher);
            ArgumentNullException.ThrowIfNull(progressReporter);
            ArgumentNullException.ThrowIfNull(retryPolicy);

            _sourceReaderFactory = sourceReaderFactory;
            _chunkReceiverFactory = chunkReceiverFactory;
            _hashCalculator = hashCalculator;
            _fileHasher = fileHasher;
            _progressReporter = progressReporter;
            _retryPolicy = retryPolicy;
        }

        public async Task<TransferReport> TransferAsync(TransferRequest request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);
            
            var checksums = new List<ChunkChecksum>();

            // This is the place where I use the factories to create the reader/receiver using the path, since
            // now I have the path in TransferRequest.
            // The file will be opened once, and used for the whole copying process.
            // If I were to use a direct interface instead of a factory and place the path in the method,
            // I would have to open and close the file up to the number of chunks that will be required for the file to be transfered.
            // And this is for both the source and destination files.
            using (ISourceReader source = _sourceReaderFactory.Create(request.SourceFilePath))
            using (IChunkReceiver receiver = _chunkReceiverFactory.Create(request.DestinationFilePath, source.Length))
            {
                long length = source.Length;
                long bytesTransferred = 0;
                int index = 0;
                
                for (long offset = 0; offset < length; offset += request.ChunkSize)
                {
                    int chunkLength = (int)Math.Min(request.ChunkSize, length - offset);
                    long chunkOffset = offset;
                    int chunkIndex = index;
                    
                    string sourceHash = string.Empty;
                    string destinationHash = string.Empty;

                    bool verified = await _retryPolicy.ExecuteAsync(async ct =>
                    {
                       FileChunk chunk = await source.ReadAsync(chunkOffset, chunkLength, ct);
                       sourceHash = _hashCalculator.ComputeHash(chunk.Data);
                       destinationHash = await receiver.ReceiveAsync(chunk, ct);
                       return sourceHash == destinationHash;
                    }, cancellationToken);

                    if (!verified)
                        throw new ChunkVerificationException(chunkIndex, chunkOffset, sourceHash, destinationHash);
                
                    checksums.Add(new ChunkChecksum(chunkIndex, chunkOffset, sourceHash));
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