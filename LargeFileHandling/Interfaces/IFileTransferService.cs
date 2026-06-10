using LargeFileHandling.Models;

namespace LargeFileHandling.Interfaces
{
    public interface IFileTransferService
    {
        void Transfer(TransferRequest request);
    }
}