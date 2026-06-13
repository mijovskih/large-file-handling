using LargeFileHandling.Models;

namespace LargeFileHandling.Interfaces
{
    public interface ISourceReader : IDisposable
    {
        long Length { get; }
        Task<FileChunk> ReadAsync(long offset, int length, CancellationToken cancellationToken);
    }
}