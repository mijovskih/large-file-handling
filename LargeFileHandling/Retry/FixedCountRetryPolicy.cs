using LargeFileHandling.Interfaces;

namespace LargeFileHandling.Retry
{
    public  sealed class FixedCountRetryPolicy : IRetryPolicy
    {
        private readonly int _maxAttempts;

        public FixedCountRetryPolicy(int maxAttempts)
        {
            if (maxAttempts < 1)
                throw new ArgumentOutOfRangeException(nameof(maxAttempts), maxAttempts, "Max attempts must be at least 1.");

            _maxAttempts = maxAttempts;
        }

        public async Task<bool> ExecuteAsync(Func<CancellationToken, Task<bool>> operation, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(operation);

            for (int attempt = 1; attempt <= _maxAttempts; attempt++)
            {
                if (await operation(cancellationToken))
                    return true;
            }

            return false;
        }
    }
}