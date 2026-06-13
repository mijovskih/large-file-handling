using System.Security.Cryptography;
using LargeFileHandling.Interfaces;

namespace LargeFileHandling.Hashing
{
    public sealed class Sha256HashCalculator : IFileHasher
    {
        public async Task<string> ComputeHashAsync(string filePath, CancellationToken cancellationToken)
        {
            await using FileStream stream = new(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: 4096, useAsync: true);

            byte[] hash = await SHA256.HashDataAsync(stream, cancellationToken);
            return Convert.ToHexString(hash).ToLowerInvariant();
        }
    }
}