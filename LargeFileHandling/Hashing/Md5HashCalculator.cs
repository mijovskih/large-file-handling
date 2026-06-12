using System.Security.Cryptography;
using LargeFileHandling.Interfaces;

namespace LargeFileHandling.Hashing
{
    public sealed class Md5HashCalculator : IHashCalculator
    {
        public string ComputeHash(ReadOnlySpan<byte> data)
            => Convert.ToHexString(MD5.HashData(data)).ToLowerInvariant();
    }
}