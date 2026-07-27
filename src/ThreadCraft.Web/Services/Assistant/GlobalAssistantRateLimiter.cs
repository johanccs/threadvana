using System.Threading.RateLimiting;

namespace ThreadCraft.Web.Services;

/// <summary>
/// One shared ceiling on how often the coach may be asked, across every learner and
/// every circuit. Without this, a single OpenRouter key has no protection against a
/// runaway loop or a burst of concurrent learners burning through quota or budget.
/// </summary>
public sealed class GlobalAssistantRateLimiter : IDisposable
{
    private readonly RateLimiter _limiter;

    public GlobalAssistantRateLimiter(AssistantOptions options)
    {
        _limiter = new SlidingWindowRateLimiter(new SlidingWindowRateLimiterOptions
        {
            PermitLimit = options.GlobalRequestsPerMinute,
            Window = TimeSpan.FromMinutes(1),
            SegmentsPerWindow = 4,
            QueueLimit = 0,
        });
    }

    /// <summary>True if a permit was available right now; never waits or queues.</summary>
    public bool TryAcquire()
    {
        using var lease = _limiter.AttemptAcquire();
        return lease.IsAcquired;
    }

    public void Dispose() => _limiter.Dispose();
}
