namespace LargeFileHandling.Exceptions
{
    public sealed class ChunkVerificationException : FileTransferException
    {
        public ChunkVerificationException(long offset, string expectedHash, string actualHash)
            : base($"Chunk verification failed at offset {offset}. Expected hash: {expectedHash}, Actual hash: {actualHash}.")
        {
            Offset = offset;
            ExpectedHash = expectedHash;
            ActualHash = actualHash;
        }

        public long Offset { get; }
        public string ExpectedHash { get; }
        public string ActualHash { get; }
    }
}