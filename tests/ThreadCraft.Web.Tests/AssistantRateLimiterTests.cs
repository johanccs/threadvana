using ThreadCraft.Web.Services;
using Xunit;

namespace ThreadCraft.Web.Tests;

/// <summary>The coach shares one OpenRouter key across every learner — these limits keep
/// one tab or one burst of traffic from burning the whole budget or starving everyone else.</summary>
public sealed class AssistantRateLimiterTests
{
    [Fact]
    public void Allows_requests_up_to_the_per_learner_limit()
    {
        var options = new AssistantOptions { PerLearnerRequestsPerMinute = 3, GlobalRequestsPerMinute = 100 };
        var limiter = new AssistantRateLimiter(options, new GlobalAssistantRateLimiter(options));

        limiter.EnsureNotRateLimited();
        limiter.EnsureNotRateLimited();
        limiter.EnsureNotRateLimited();

        var ex = Assert.Throws<AssistantException>(limiter.EnsureNotRateLimited);
        Assert.Contains("faster than the coach can keep up", ex.Message);
    }

    [Fact]
    public void Global_limit_applies_across_separate_learner_instances()
    {
        var options = new AssistantOptions { PerLearnerRequestsPerMinute = 100, GlobalRequestsPerMinute = 2 };
        var global = new GlobalAssistantRateLimiter(options);
        var learnerA = new AssistantRateLimiter(options, global);
        var learnerB = new AssistantRateLimiter(options, global);

        learnerA.EnsureNotRateLimited();
        learnerB.EnsureNotRateLimited();

        var ex = Assert.Throws<AssistantException>(learnerA.EnsureNotRateLimited);
        Assert.Contains("a lot of questions from everyone", ex.Message);
    }
}
