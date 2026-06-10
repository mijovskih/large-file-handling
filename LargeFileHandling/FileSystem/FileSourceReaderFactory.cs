using LargeFileHandling.Interfaces;

namespace LargeFileHandling.FileSystem
{
    public sealed class FileSourceReaderFactory : ISourceReaderFactory
    {
        public ISourceReader Create(string path) => new FileSourceReader(path);
    }
}