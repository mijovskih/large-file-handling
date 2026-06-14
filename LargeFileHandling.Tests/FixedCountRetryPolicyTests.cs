using LargeFileHandling.Interfaces;
using LargeFileHandling.Retry;

namespace LargeFileHandling.Tests
{
    [TestFixture]
    public class FixedCountRetryPolicyTests
    {
        [Test]
        public async Task ExecuteAsync_SucceedsOnSecondTry_ReturnsTrueAndStops()
        {
            int calls = 0;
            IRetryPolicy policy = new FixedCountRetryPolicy(maxAttempts: 3);

            bool result = await policy.ExecuteAsync(_ =>
            {
                calls++;
                return Task.FromResult(calls == 2); // Will fail on first, but succeed on second.
            }, CancellationToken.None);

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.True);
                Assert.That(calls, Is.EqualTo(2));
            });
        }

        [Test]
        public async Task ExecuteAsync_AlwaysFails_ReturnsFalseAfterMaxAttempts()
        {
            int calls = 0;
            IRetryPolicy policy = new FixedCountRetryPolicy(maxAttempts: 3);

            bool result = await policy.ExecuteAsync(_ =>
            {
                calls++;
                return Task.FromResult(false);
            }, CancellationToken.None);

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.False);
                Assert.That(calls, Is.EqualTo(3));
            });
        }

        [Test]
        public void Constructor_MaxAttemptsBelowOne_Throw()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new FixedCountRetryPolicy(0));
        }
    }
}