namespace LargeFileHandling.Exceptions
{
    public class FileTransferException : Exception
    {
        public FileTransferException(string message) : base(message) { }

        public FileTransferException(string message, Exception innerException) : base(message, innerException) { }
    }
}