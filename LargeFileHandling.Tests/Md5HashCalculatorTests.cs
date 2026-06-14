using System.Text;
using LargeFileHandling.Hashing;
using LargeFileHandling.Interfaces;

namespace LargeFileHandling.Tests
{
    [TestFixture]
    public class Md5HashCalculatorTests
    {
        private readonly IHashCalculator _hashCalculator = new Md5HashCalculator();

        [Test]
        public void ComputeHash_KnownInput_ReturnsExpectedMd5()
        {
            byte[] data = Encoding.UTF8.GetBytes("abc");

            string hash = _hashCalculator.ComputeHash(data);

            Assert.That(hash, Is.EqualTo("900150983cd24fb0d6963f7d28e17f72"));
        }

        [Test]
        public void ComputeHash_EmptyInput_ReturnsKnownEmptyMd5()
        {
            string hash = _hashCalculator.ComputeHash(ReadOnlySpan<byte>.Empty);

            Assert.That(hash, Is.EqualTo("d41d8cd98f00b204e9800998ecf8427e"));
        }
    }
}