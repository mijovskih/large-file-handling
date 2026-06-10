namespace LargeFileHandling.Interfaces
{
    public interface ISourceReaderFactory
    {
        ISourceReader Create(string path);
    }
}