using System.Threading;
using System.Threading.Tasks;

public static class Solution
{
    public sealed class RateLimiter
    {
        private readonly int _maxPerSecond;
        // TODO: add fields — a SemaphoreSlim, a Timer, a counter for tokens

        public RateLimiter(int maxPerSecond)
        {
            _maxPerSecond = maxPerSecond;
            // TODO: start a timer and initialize your state
        }

        public Task<bool> TryActionAsync()
        {
            // TODO: if a token is available, consume it and return true;
            // otherwise return false. Use SemaphoreSlim to serialise access.
            return Task.FromResult(true);
        }
    }
}
