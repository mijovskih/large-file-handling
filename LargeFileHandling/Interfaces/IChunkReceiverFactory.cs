namespace LargeFileHandling.Interfaces
{
    public interface IChunkReceiverFactory
    {
        IChunkReceiver Create(string path, long totalLength);
    }
}