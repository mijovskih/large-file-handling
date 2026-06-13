using LargeFileHandling.Models;

namespace LargeFileHandling.Interfaces
{
    public interface IFileTransferService
    {
        TransferReport Transfer(TransferRequest request);
    }
}