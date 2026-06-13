namespace LargeFileHandling.Models
{
    public sealed class TransferReport
    {
        public TransferReport(IReadOnlyList<ChunkChecksum> chunkChecksums, string sourceFileHash, string destinationFileHash)
        {
            ArgumentNullException.ThrowIfNull(chunkChecksums);
            ArgumentException.ThrowIfNullOrWhiteSpace(sourceFileHash);
            ArgumentException.ThrowIfNullOrWhiteSpace(destinationFileHash);

            ChunkChecksums = chunkChecksums;
            SourceFileHash = sourceFileHash;
            DestinationFileHash = destinationFileHash;
        }

        public IReadOnlyList<ChunkChecksum> ChunkChecksums { get; }
        public string SourceFileHash { get; }
        public string DestinationFileHash { get; }

        public bool HashesMatch => string.Equals(SourceFileHash, DestinationFileHash, StringComparison.Ordinal);
    }
}