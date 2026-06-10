using LargeFileHandling.Models;

namespace LargeFileHandling.Interfaces
{
    public interface IChunkReceiver : IDisposable
    {
        void Receive(FileChunk chunk);
    }
}