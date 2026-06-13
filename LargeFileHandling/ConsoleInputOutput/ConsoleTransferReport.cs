using LargeFileHandling.Models;

namespace LargeFileHandling.ConsoleInputOutput
{
    public sealed class ConsoleTransferReport
    {
        public void ShowReport(TransferReport report)
        {
            ArgumentNullException.ThrowIfNull(report);

            Console.WriteLine();
            Console.WriteLine("Chunk checksums: ");

            foreach (ChunkChecksum checksum in report.ChunkChecksums)
            {
                Console.WriteLine($"{checksum.Index + 1}) position = {checksum.Offset}, hash = {checksum.Md5Hash}");
            }
        }
    }
}