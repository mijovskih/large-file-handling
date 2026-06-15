using LargeFileHandling.Exceptions;
using LargeFileHandling.FileSystem;
using LargeFileHandling.Hashing;
using LargeFileHandling.ConsoleInputOutput;
using LargeFileHandling.Interfaces;
using LargeFileHandling.Models;
using LargeFileHandling.Services;
using LargeFileHandling.Retry;

// This is the only place in the application where I chose concrete classes to be called and wired to their interfaces.
// Every other file depeneds on interfaces instead.
// Everything in this class is wired manually instead of using the DI container.
// It's still dependency injection, but kept a bit simpler since it's a small application.
try
{
    // The reason why I chose an interface as the variable and not the class:
    // Anything I pass these into will only see the abstraction, and won't depend on a specific implementation.
    // Swapping a class here will not break the rest of the code.
    IHashCalculator hashCalculator = new Md5HashCalculator();
    IFileHasher fileHasher = new Sha256HashCalculator();

    // I chose factories instead of the reader/receiver directly because they need a file path to be created.
    // Since the path is not known to us at startup, the user first must type it in, which happens after this is called.
    // Instead, I inject the factories that know hwo to build a reader/receiver later, when we know the path.
    // The key part is that their methods return the interface, so they are still only abstractions.
    ISourceReaderFactory sourceReaderFactory = new FileSourceReaderFactory();
    IChunkReceiverFactory chunkReceiverFactory = new FileChunkReceiverFactory(hashCalculator);

    IProgressReporter progressReporter = new ConsoleProgressReport();

    const int maxChunkAttempts = 3;
    IRetryPolicy retryPolicy = new FixedCountRetryPolicy(maxChunkAttempts);

    IFileTransferService transferService = new FileTransferService(
        sourceReaderFactory, 
        chunkReceiverFactory, 
        hashCalculator, 
        fileHasher, 
        progressReporter, 
        retryPolicy);

    var inputReader = new ConsoleInputReader();
    TransferRequest request = inputReader.Read();
    Console.WriteLine($"Copying {request.SourceFilePath} to {request.DestinationFilePath} with chunk size {request.ChunkSize} bytes...");
    
    TransferReport report = await transferService.TransferAsync(request, CancellationToken.None);
    var presenter = new ConsoleTransferReport();
    presenter.ShowReport(report);

    Console.WriteLine("File transfer completed.");

    return 0;
}
// This is the exception handler, a known error will get a short message and an exit code 1.
// An unexpected exception will get an exit code 2, which will be easier to spot and filter out.
catch (FileTransferException ex)
{
    Console.Error.WriteLine($"File transfer failed: {ex.Message}");
    return 1;
}
catch (Exception ex) when (ex is FileNotFoundException || ex is DirectoryNotFoundException || ex is UnauthorizedAccessException || ex is IOException)
{
    Console.Error.WriteLine($"Error: {ex.Message}");
    return 1;
}
catch (Exception ex)
{
    Console.Error.WriteLine($"An unexpected error occurred: {ex.Message}");
    return 2;
}