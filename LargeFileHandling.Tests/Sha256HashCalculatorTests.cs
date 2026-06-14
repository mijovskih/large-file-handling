using System.Text;
using LargeFileHandling.Hashing;
using LargeFileHandling.Interfaces;

namespace LargeFileHandling.Tests
{
    [TestFixture]
    public class Sha256HashCalculatorTests
    {
        private readonly IFileHasher _fileHasher = new Sha256HashCalculator();

        [Test]
        public async Task ComputeHashAsync_KnownContent_ReturnsExpectedSha256()
        {
            string path = Path.GetTempFileName();

            try
            {
                await File.WriteAllBytesAsync(path, Encoding.UTF8.GetBytes("abc"));

                string hash = await _fileHasher.ComputeHashAsync(path, CancellationToken.None);

                Assert.That(hash, Is.EqualTo("ba7816bf8f01cfea414140de5dae2223b00361a396177a9cb410ff61f20015ad"));
            }
            finally
            {
                File.Delete(path);
            }
        }

        [Test]
        public async Task ComputeHashAsync_EmptyFile_ReturnsKnownEmptySha256()
        {
            string path = Path.GetTempFileName(); // This will eventually create an empty file.

            try
            {
                string hash = await _fileHasher.ComputeHashAsync(path, CancellationToken.None);

                Assert.That(hash, Is.EqualTo("e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855"));
            }
            finally
            {
                File.Delete(path);
            }
        }
    }
}