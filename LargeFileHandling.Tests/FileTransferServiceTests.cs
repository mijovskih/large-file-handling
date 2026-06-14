using LargeFileHandling.Exceptions;
using LargeFileHandling.Hashing;
using LargeFileHandling.Interfaces;
using LargeFileHandling.Models;
using LargeFileHandling.Retry;
using LargeFileHandling.Services;
using Moq;

namespace LargeFileHandling.Tests
{
    [TestFixture]
    public class FileTransferServiceTests
    {
        private static readonly IHashCalculator Md5 = new Md5HashCalculator();

        [Test]
        public async Task TransferAsync_ValidTransfer_CopiesBytesAndRecordsOrderedChecksums()
        {
            byte[] source = CreateBytes(2500);
            byte[] destination = new byte[source.Length];

            Mock<IChunkReceiver> receiver = CreateReceiver(destination, failuresToInject: 0);
            FileTransferService service = CreateService(source, receiver.Object);
            var request = new TransferRequest("source", "dest", chunkSize: 1000);

            TransferReport report = await service.TransferAsync(request, CancellationToken.None);

            Assert.Multiple(() =>
            {
                Assert.That(destination, Is.EqualTo(source));
                Assert.That(report.ChunkChecksums, Has.Count.EqualTo(3));
            });

            Assert.Multiple(() =>
            {
                Assert.That(report.ChunkChecksums[0].Offset, Is.EqualTo(0));
                Assert.That(report.ChunkChecksums[1].Offset, Is.EqualTo(1000));
                Assert.That(report.ChunkChecksums[2].Offset, Is.EqualTo(2000));   // short final chunk
                Assert.That(report.HashesMatch, Is.True);
            });
        }

        [Test]
        public async Task TransferAsync_EmptySource_ProducesNoChecksums()
        {
            byte[] source = Array.Empty<byte>();

            Mock<IChunkReceiver> receiver = CreateReceiver(Array.Empty<byte>(), failuresToInject: 0);

            FileTransferService service = CreateService(source, receiver.Object);
            var request = new TransferRequest("source", "dest", chunkSize: 1000);

            TransferReport report = await service.TransferAsync(request, CancellationToken.None);

            Assert.That(report.ChunkChecksums, Is.Empty);
        }

        [Test]
        public async Task TransferAsync_DestinationHashWrongOnce_RetriesThenSucceeds()
        {
            byte[] source = CreateBytes(500);
            byte[] destination = new byte[source.Length];
  
            Mock<IChunkReceiver> receiver = CreateReceiver(destination, failuresToInject: 1);
            FileTransferService service = CreateService(source, receiver.Object);
            var request = new TransferRequest("source", "dest", chunkSize: 1000);

            TransferReport report = await service.TransferAsync(request, CancellationToken.None);
            Assert.Multiple(() =>
            {
                Assert.That(report.ChunkChecksums, Has.Count.EqualTo(1));
                Assert.That(destination, Is.EqualTo(source));
            });
            receiver.Verify(r => r.ReceiveAsync(It.IsAny<FileChunk>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
        }

        [Test]
        public void TransferAsync_DestinationHashAlwaysWrong_ThrowsChunkVerificationException()
        {
            byte[] source = CreateBytes(500);

            Mock<IChunkReceiver> receiver = CreateReceiver(new byte[source.Length], failuresToInject: 99); 
            FileTransferService service = CreateService(source, receiver.Object);
            var request = new TransferRequest("source", "dest", chunkSize: 1000);

            Assert.That(async () => 
                await service.TransferAsync(request, CancellationToken.None),
                Throws.TypeOf<ChunkVerificationException>());
        }

        #region Helpers

        private static Mock<IChunkReceiver> CreateReceiver(byte[] destination, int failuresToInject)
        {
            int calls = 0;
            var receiver = new Mock<IChunkReceiver>();
            receiver
                .Setup(r => r.ReceiveAsync(It.IsAny<FileChunk>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((FileChunk chunk, CancellationToken _) =>
                {
                    calls++;
                    chunk.Data.CopyTo(destination, (int)chunk.Offset);
                    return calls <= failuresToInject ? "wronghash" : Md5.ComputeHash(destination.AsSpan((int)chunk.Offset, chunk.Length));
                });
            return receiver;
        }

        private static FileTransferService CreateService(byte[] source, IChunkReceiver receiver)
        {
            var sourceReader = new Mock<ISourceReader>();
            sourceReader.SetupGet(r => r.Length).Returns(source.Length);
            sourceReader
                .Setup(r => r.ReadAsync(It.IsAny<long>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((long offset, int length, CancellationToken _) =>
                    new FileChunk(offset, source.AsSpan((int)offset, length).ToArray()));

            var sourceFactory = new Mock<ISourceReaderFactory>();
            sourceFactory
                .Setup(f => f.Create(It.IsAny<string>()))
                .Returns(sourceReader.Object);

            var receiverFactory = new Mock<IChunkReceiverFactory>();
            receiverFactory
                .Setup(f => f.Create(It.IsAny<string>(), It.IsAny<long>()))
                .Returns(receiver);

            var fileHasher = new Mock<IFileHasher>();
            fileHasher
                .Setup(h => h.ComputeHashAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync("whole-file-hash");

            var progress = new Mock<IProgressReporter>();

            return new FileTransferService(
                sourceFactory.Object, 
                receiverFactory.Object, 
                Md5, 
                fileHasher.Object, 
                progress.Object, 
                new FixedCountRetryPolicy(maxAttempts: 3));
        }

        private static byte[] CreateBytes(int count)
        {
            var data = new byte[count];
            for (int i = 0; i < count; i++)
                data[i] = (byte)(i % 251);
            return data;
        }

        #endregion
    }
}