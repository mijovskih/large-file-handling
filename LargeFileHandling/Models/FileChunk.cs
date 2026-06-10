namespace LargeFileHandling.Models
{
    public sealed class FileChunk
    {
        public FileChunk(long offset, byte[] data)
        {
            if (offset < 0)
                throw new ArgumentOutOfRangeException(nameof(offset), "Offset cannot be negative.");

            if (data == null || data.Length == 0)
                throw new ArgumentException("Data cannot be null or empty.", nameof(data));

            Offset = offset;
            Data = data;
        }

        public long Offset { get; } // At first I was thinking of using int, but for large files, long is more appropriate to handle files larger than 2GB.
        public byte[] Data { get; }
        public int Length => Data.Length; // I will let this be an integer for now, 
                                        // because a chunk will be a small portion of the file. 
                                        // For now, I think it's unlikely that a chunk will be larger than 2GB.
    }
}