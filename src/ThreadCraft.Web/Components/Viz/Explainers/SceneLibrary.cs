namespace ThreadCraft.Web.Components.Viz.Explainers;

/// <summary>
/// Built-in animated explainers. A lesson picks one explicitly via the
/// "explainer:" front-matter key; visualizations with a clearly matching
/// concept get one by default via <see cref="DefaultForVisualization"/>.
/// </summary>
public static partial class SceneLibrary
{
    public static ExplainerScene? Get(string? id) => id switch
    {
        "thread-basics" => ThreadBasics(),
        "thread-join" => ThreadJoin(),
        "thread-pool" => ThreadPool(),
        "semaphore" => Semaphore(),
        "async-state-machine" => AsyncStateMachine(),
        "race-interleaving" => RaceInterleaving(),
        "foreground-background" => ForegroundBackground(),
        "cancellation" => Cancellation(),
        "deadlock" => Deadlock(),
        "lock-key" => LockKey(),
        "channel" => ChannelScene(),
        "thread-local" => ThreadLocal(),
        "event-gate" => EventGate(),
        _ => null
    };

    /// <summary>Fallback when a lesson has no explicit explainer. "thread-timeline" is used
    /// by many different concepts, so it gets NO default (a wrong animation is worse than
    /// none) — those lessons should set "explainer:" explicitly.</summary>
    public static string? DefaultForVisualization(string? visualizationId) => visualizationId switch
    {
        "thread-pool" => "thread-pool",
        "semaphore" => "semaphore",
        "async-activity" => "async-state-machine",
        _ => null
    };
}
