namespace ThreadCraft.Core.Tracing;

/// <summary>
/// One thing that happened while demo/exercise code ran.
/// Emitted by the injected Trace helper as JSON-lines, parsed by the host,
/// and rendered live by the visualization components.
/// </summary>
public sealed record TraceEvent(
    /// <summary>Milliseconds since the run started.</summary>
    long TimestampMs,
    int ThreadId,
    string ThreadName,
    /// <summary>One of TraceKinds.*</summary>
    string Kind,
    /// <summary>Short human-readable description, e.g. "Waiting for semaphore".</summary>
    string Label);

/// <summary>Well-known trace event kinds consumed by the visualization components.</summary>
public static class TraceKinds
{
    public const string ThreadStart = "thread-start";
    public const string ThreadEnd = "thread-end";
    public const string WorkStart = "work-start";
    public const string WorkEnd = "work-end";
    public const string WaitStart = "wait-start";
    public const string WaitEnd = "wait-end";
    public const string LockAcquire = "lock-acquire";
    public const string LockRelease = "lock-release";
    public const string SemaphoreEnter = "semaphore-enter";
    public const string SemaphoreExit = "semaphore-exit";
    public const string PoolQueued = "pool-queued";
    public const string PoolDequeued = "pool-dequeued";
    public const string Message = "message";

    public static readonly IReadOnlySet<string> All = new HashSet<string>
    {
        ThreadStart, ThreadEnd, WorkStart, WorkEnd, WaitStart, WaitEnd,
        LockAcquire, LockRelease, SemaphoreEnter, SemaphoreExit,
        PoolQueued, PoolDequeued, Message
    };
}
