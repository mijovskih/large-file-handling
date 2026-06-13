namespace LargeFileHandling.Interfaces
{
    public interface IProgressReporter
    {
        void ShowProgress(long bytesCompleted, long totalBytes);
    }
}