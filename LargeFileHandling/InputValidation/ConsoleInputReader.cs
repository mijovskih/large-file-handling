using LargeFileHandling.Models;

namespace LargeFileHandling.InputValidation
{
    public sealed class ConsoleInputReader
    {
        private const int DefaultChunkSize = 1024 * 1024; // 1 MB

        public TransferRequest Read()
        {
            string sourceFilePath = ReadSourceFilePath();
            string destinationFilePath = ReadDestinationFilePath(sourceFilePath);
            return new TransferRequest(sourceFilePath, destinationFilePath, DefaultChunkSize);
        }

        private static string ReadSourceFilePath()
        {
            while (true)
            {
                Console.Write("Enter the source file path: ");
                string? input = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(input))
                {
                    Console.WriteLine("Source file path cannot be empty. Please try again.");
                    continue;
                }

                string path = input.Trim();
                if (!File.Exists(path))
                {
                    Console.WriteLine("The specified source file does not exist. Please try again.");
                    continue;
                }

                return path;
            }
        }

        private static string ReadDestinationFilePath(string sourceFilePath)
        {
            while (true)
            {
                Console.Write("Enter the destination folder path: ");
                string? input = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(input))
                {
                    Console.WriteLine("Destination folder path cannot be empty. Please try again.");
                    continue;
                }

                string destinationDirectory = input.Trim();
                Directory.CreateDirectory(destinationDirectory); // Ensure the directory exists

                string destinationFilePath = Path.Combine(destinationDirectory, Path.GetFileName(sourceFilePath));

                if (PathsReferToSameFile(sourceFilePath, destinationFilePath))
                {
                    Console.WriteLine("The destination path cannot be the same as the source file. Please try again.");
                    continue;
                }

                return destinationFilePath;
            }
        }

        private static bool PathsReferToSameFile(string source, string destination) =>
            string.Equals(Path.GetFullPath(source), Path.GetFullPath(destination), StringComparison.OrdinalIgnoreCase);
    }
}