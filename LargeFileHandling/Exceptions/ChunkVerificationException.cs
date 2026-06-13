namespace LargeFileHandling.Exceptions
{
    public sealed class ChunkVerificationException : FileTransferException
    {
        public ChunkVerificationException(int index, long offset, string expectedHash, string actualHash)
            : base($"Chunk verification failed at offset {offset}. Expected hash: {expectedHash}, Actual hash: {actualHash}.")
        {
            Index = index;
            Offset = offset;
            ExpectedHash = expectedHash;
            ActualHash = actualHash;
        }

        public int Index { get; }
        public long Offset { get; }
        public string ExpectedHash { get; }
        public string ActualHash { get; }
    }
}