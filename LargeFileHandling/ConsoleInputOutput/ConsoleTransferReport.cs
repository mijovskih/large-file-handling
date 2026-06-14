using LargeFileHandling.Models;

namespace LargeFileHandling.ConsoleInputOutput
{
    public sealed class ConsoleTransferReport
    {
        public void ShowReport(TransferReport report)
        {
            ArgumentNullException.ThrowIfNull(report);

            Console.WriteLine();

            if(!report.ChunkChecksums.Any())
                Console.WriteLine("File is empty. No chunk checksums have been computed.");
            else
            {
                Console.WriteLine("Chunk checksums: ");
                foreach (ChunkChecksum checksum in report.ChunkChecksums)
                {
                    Console.WriteLine($"{checksum.Index + 1}) position = {checksum.Offset}, hash = {checksum.Md5Hash}");
                }
            }

            Console.WriteLine();
            Console.WriteLine($"Source SHA-256: {report.SourceFileHash}");
            Console.WriteLine($"Destination SHA-256: {report.DestinationFileHash}");
            Console.WriteLine(report.HashesMatch ? "File checksums match." : "ERROR: File checksums do not match!");
        }
    }
}