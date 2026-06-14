namespace LargeFileHandling.Interfaces
{
    public interface IRetryPolicy
    {
        // Bool from the function will return true = succeeded, false = try again. My idea is that false will retry it succeeds, or it terminates.
        Task<bool> ExecuteAsync(Func<CancellationToken, Task<bool>> operation, CancellationToken cancellationToken);
    }
}