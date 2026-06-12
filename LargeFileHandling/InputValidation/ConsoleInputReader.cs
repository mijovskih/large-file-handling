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

                // During testing, I ran into the issue where I provided a file path instead of a folder.
                // This checks prevents an exception being thrown when replicating that.
                if (File.Exists(destinationDirectory))
                {
                    Console.WriteLine("The specified destination path is a file. Please enter a valid directory path.");
                    continue;
                }

                Directory.CreateDirectory(destinationDirectory); // Ensure the directory exists

                string destinationFilePath = Path.Combine(destinationDirectory, Path.GetFileName(sourceFilePath));

                // During testing, I ran into the issue where a folder existed with the exact name as the file I was trying to create.
                // This check prevents that, but doesn't forbid the user to create a folder with the same name (e.g. file.txt).
                if (Directory.Exists(destinationFilePath))
                {
                    Console.WriteLine("A folder already exists where the file should be created. Please choose another destination.");
                    continue;
                }

                // Source and destination paths should not be the same.
                if (PathsReferToSameFile(sourceFilePath, destinationFilePath))
                {
                    Console.WriteLine("The destination path cannot be the same as the source file. Please try again.");
                    continue;
                }

                return destinationFilePath;
            }
        }

        // I can either use .ToUpper/ToLower or StringComparison.OrdinalIgnoreCase. I prefer the second option.
        // Strings are immutable, so using the first option would create a new string in memory, unlike the second option.
        // These are small strings, so performance will not really be an issue, but it's a good opportunity to show this.
        private static bool PathsReferToSameFile(string source, string destination) =>
            string.Equals(Path.GetFullPath(source), Path.GetFullPath(destination), StringComparison.OrdinalIgnoreCase);
    }
}