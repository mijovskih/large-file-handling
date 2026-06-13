namespace LargeFileHandling.Models
{
    public sealed class TransferReport
    {
        public TransferReport(IReadOnlyList<ChunkChecksum> chunkChecksums)
        {
            ArgumentNullException.ThrowIfNull(chunkChecksums);
            ChunkChecksums = chunkChecksums;
        }

        public IReadOnlyList<ChunkChecksum> ChunkChecksums { get; }
    }
}