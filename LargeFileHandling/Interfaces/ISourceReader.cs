using LargeFileHandling.Models;

namespace LargeFileHandling.Interfaces
{
    public interface ISourceReader : IDisposable
    {
        long Length { get; }
        FileChunk Read(long offset, int length);
    }
}