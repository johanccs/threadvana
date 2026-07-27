using System.Threading.RateLimiting;

namespace ThreadCraft.Web.Services;

/// <summary>
/// Caps how often one learner (one Blazor circuit — this is registered scoped) can
/// ask the coach, on top of the app-wide <see cref="GlobalAssistantRateLimiter"/>.
/// Stops one open tab from starving everyone else or running up the OpenRouter bill
/// on its own, e.g. via a scripted loop or a learner mashing "Ask".
/// </summary>
public sealed class AssistantRateLimiter : IDisposable
{
    private readonly GlobalAssistantRateLimiter _global;
    private readonly RateLimiter _perLearner;

    public AssistantRateLimiter(AssistantOptions options, GlobalAssistantRateLimiter global)
    {
        _global = global;
        _perLearner = new SlidingWindowRateLimiter(new SlidingWindowRateLimiterOptions
        {
            PermitLimit = options.PerLearnerRequestsPerMinute,
            Window = TimeSpan.FromMinutes(1),
            SegmentsPerWindow = 4,
            QueueLimit = 0,
        });
    }

    /// <summary>Throws <see cref="AssistantException"/> with a learner-friendly message if over limit.</summary>
    public void EnsureNotRateLimited()
    {
        if (!_global.TryAcquire())
        {
            throw new AssistantException(
                "The coach is getting a lot of questions from everyone right now. Wait a minute and try again.");
        }

        using var lease = _perLearner.AttemptAcquire();
        if (!lease.IsAcquired)
        {
            throw new AssistantException(
                "You're asking questions faster than the coach can keep up. Wait a bit and try again.");
        }
    }

    public void Dispose() => _perLearner.Dispose();
}
