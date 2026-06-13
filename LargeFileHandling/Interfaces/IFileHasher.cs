namespace LargeFileHandling.Interfaces
{
    public interface IFileHasher
    {
        Task<string> ComputeHashAsync(string filePath, CancellationToken cancellationToken);
    }
}