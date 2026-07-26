namespace ThreadCraft.Core.Progress;

/// <summary>How far a learner got with one lesson.</summary>
public sealed record ProgressRecord(
    string LessonId,
    int Attempts,
    bool ExercisePassed,
    DateTimeOffset LastAttemptAt,
    DateTimeOffset? PassedAt);

/// <summary>
/// Persists learner progress. Implemented in ThreadCraft.Web with EF Core + SQLite.
/// Scoped per app (single-learner local deployment); a UserId can be added later
/// without changing call sites if multi-user support is ever needed.
/// </summary>
public interface IProgressStore
{
    Task<ProgressRecord?> GetAsync(string lessonId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ProgressRecord>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>Records one submission attempt (increments Attempts, updates LastAttemptAt).</summary>
    Task<ProgressRecord> MarkAttemptAsync(string lessonId, CancellationToken cancellationToken = default);

    /// <summary>Records a passed exercise (also counts as an attempt). Idempotent.</summary>
    Task<ProgressRecord> MarkPassedAsync(string lessonId, CancellationToken cancellationToken = default);
}
