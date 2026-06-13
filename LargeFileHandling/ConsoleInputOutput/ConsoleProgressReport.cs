using LargeFileHandling.Interfaces;

namespace LargeFileHandling.ConsoleInputOutput
{
    public sealed class ConsoleProgressReport : IProgressReporter
    {
        public void ShowProgress(long bytesCompleted, long totalBytes)
        {
            // totalBytes == 0 ? 100 will guard the scenario where we provide an empty file to be copied.
            double percentage = totalBytes == 0 ? 100 : (double)bytesCompleted / totalBytes * 100;

            // \r = move the cursor to the start; :F1 = 1 decimal; :N0 = thousands separator.
            Console.Write($"\rProgress: {percentage:F1}% ({bytesCompleted:N0} / {totalBytes:N0} bytes)");

            // If progress is done, write a new line to terminate the progress output.
            if (bytesCompleted >= totalBytes)
                Console.WriteLine();
        }
    }
}