using LargeFileHandling.Exceptions;
using LargeFileHandling.FileSystem;
using LargeFileHandling.Hashing;
using LargeFileHandling.ConsoleInputOutput;
using LargeFileHandling.Interfaces;
using LargeFileHandling.Models;
using LargeFileHandling.Services;

try
{
    IHashCalculator hashCalculator = new Md5HashCalculator();
    ISourceReaderFactory sourceReaderFactory = new FileSourceReaderFactory();
    IChunkReceiverFactory chunkReceiverFactory = new FileChunkReceiverFactory(hashCalculator);
    IFileTransferService transferService = new FileTransferService(sourceReaderFactory, chunkReceiverFactory, hashCalculator);

    var inputReader = new ConsoleInputReader();

    TransferRequest request = inputReader.Read();

    Console.WriteLine($"Copying {request.SourceFilePath} to {request.DestinationFilePath} with chunk size {request.ChunkSize} bytes...");
    
    TransferReport report = transferService.Transfer(request);
    var presenter = new ConsoleTransferReport();
    presenter.ShowReport(report);

    Console.WriteLine("File transfer completed.");

    return 0;
}
catch (FileTransferException ex)
{
    Console.Error.WriteLine($"File transfer failed: {ex.Message}");
    return 1;
}
catch (Exception ex) when (ex is FileNotFoundException  || ex is DirectoryNotFoundException || ex is UnauthorizedAccessException || ex is IOException)
{
    Console.Error.WriteLine($"Error: {ex.Message}");
    return 1;
}
catch (Exception ex)
{
    Console.Error.WriteLine($"An unexpected error occurred: {ex.Message}");
    return 2;
}