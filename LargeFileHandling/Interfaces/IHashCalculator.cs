namespace LargeFileHandling.Interfaces
{
    public interface IHashCalculator
    {
        string ComputeHash(ReadOnlySpan<byte> data);
    }
}