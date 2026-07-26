using System.Threading;
using System.Threading.Tasks;

public static class Solution
{
    public sealed class RateLimiter
    {
        private readonly SemaphoreSlim _gate = new(1, 1);
        private readonly Timer _timer;
        private volatile int _tokens;

        public RateLimiter(int maxPerSecond)
        {
            _tokens = 0; // Start empty — tokens accumulate from the timer
            var interval = 1000 / maxPerSecond;
            _timer = new Timer(_ => Interlocked.Add(ref _tokens, 1), null, interval, interval);
        }

        public async Task<bool> TryActionAsync()
        {
            await _gate.WaitAsync();
            try
            {
                if (Volatile.Read(ref _tokens) <= 0) return false;
                Interlocked.Decrement(ref _tokens);
                return true;
            }
            finally
            {
                _gate.Release();
            }
        }
    }
}
