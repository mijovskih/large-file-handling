using LargeFileHandling.Models;

namespace LargeFileHandling.Interfaces
{
    public interface IFileTransferService
    {
        Task<TransferReport> TransferAsync(TransferRequest request, CancellationToken cancellationToken);
    }
}