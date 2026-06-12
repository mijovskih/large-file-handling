using LargeFileHandling.Models;

namespace LargeFileHandling.Interfaces
{
    public interface IChunkReceiver : IDisposable
    {
        string Receive(FileChunk chunk);
    }
}