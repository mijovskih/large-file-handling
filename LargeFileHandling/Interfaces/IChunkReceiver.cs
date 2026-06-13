using LargeFileHandling.Models;

namespace LargeFileHandling.Interfaces
{
    public interface IChunkReceiver : IDisposable
    {
        Task<string> ReceiveAsync(FileChunk chunk, CancellationToken cancellationToken);
    }
}