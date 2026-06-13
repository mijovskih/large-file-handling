namespace LargeFileHandling.Models
{
    public sealed class ChunkChecksum
    {
        public ChunkChecksum(int index, long offset, string md5Hash)
        {
            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(index), index, "Index must be a positive number.");
            if (offset < 0)
                throw new ArgumentOutOfRangeException(nameof(offset), offset, "Offset must be a positive number.");
            ArgumentException.ThrowIfNullOrWhiteSpace(md5Hash);

            Index = index;
            Offset = offset;
            Md5Hash = md5Hash;
        }

        public int Index { get; }
        public long Offset { get; }
        public string Md5Hash { get; }
    }
}