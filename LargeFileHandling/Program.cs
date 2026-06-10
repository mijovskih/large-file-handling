using LargeFileHandling.FileSystem;
using LargeFileHandling.InputValidation;
using LargeFileHandling.Interfaces;
using LargeFileHandling.Models;
using LargeFileHandling.Services;

ISourceReaderFactory sourceReaderFactory = new FileSourceReaderFactory();
IChunkReceiverFactory chunkReceiverFactory = new FileChunkReceiverFactory();
IFileTransferService transferService = new FileTransferService(sourceReaderFactory, chunkReceiverFactory);

var inputReader = new ConsoleInputReader();

TransferRequest request = inputReader.Read();

Console.WriteLine($"Copying {request.SourceFilePath} to {request.DestinationFilePath} with chunk size {request.ChunkSize} bytes...");
transferService.Transfer(request);
Console.WriteLine("File transfer completed.");