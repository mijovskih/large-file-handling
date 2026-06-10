namespace LargeFileHandling.Models
{
    public sealed class TransferRequest
    {
        public TransferRequest(string sourceFilePath, string destinationFilePath, int chunkSize)
        {
            if (string.IsNullOrWhiteSpace(sourceFilePath))
                throw new ArgumentException("Source file path cannot be null or empty.", nameof(sourceFilePath));

            if (string.IsNullOrWhiteSpace(destinationFilePath))
                throw new ArgumentException("Destination file path cannot be null or empty.", nameof(destinationFilePath));

            if (chunkSize <= 0)                
                throw new ArgumentOutOfRangeException(nameof(chunkSize), "Chunk size must be a positive integer.");

            SourceFilePath = sourceFilePath;
            DestinationFilePath = destinationFilePath;
            ChunkSize = chunkSize;
        }

        public string SourceFilePath { get; }
        public string DestinationFilePath { get; }
        public int ChunkSize { get; }
    }
}