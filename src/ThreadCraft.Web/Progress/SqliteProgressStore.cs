using Microsoft.EntityFrameworkCore;
using ThreadCraft.Core.Progress;

namespace ThreadCraft.Web.Progress;

/// <summary>
/// Stores learner progress in SQLite via EF Core. Thread-safe: every operation
/// creates its own short-lived <see cref="ProgressDbContext"/> from the factory,
/// so concurrent pages never share a context instance.
/// </summary>
public sealed class SqliteProgressStore : IProgressStore
{
    private readonly IDbContextFactory<ProgressDbContext> _contextFactory;

    public SqliteProgressStore(IDbContextFactory<ProgressDbContext> contextFactory)
        => _contextFactory = contextFactory;

    public async Task<ProgressRecord?> GetAsync(string lessonId, CancellationToken cancellationToken = default)
    {
        await using var db = await _contextFactory.CreateDbContextAsync(cancellationToken);
        var entity = await db.ProgressRecords.FindAsync([lessonId], cancellationToken);
        return entity is null ? null : ToRecord(entity);
    }

    public async Task<IReadOnlyList<ProgressRecord>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        await using var db = await _contextFactory.CreateDbContextAsync(cancellationToken);
        var entities = await db.ProgressRecords.AsNoTracking().ToListAsync(cancellationToken);
        return entities.Select(ToRecord).ToList();
    }

    public Task<ProgressRecord> MarkAttemptAsync(string lessonId, CancellationToken cancellationToken = default)
        => UpdateAsync(lessonId, markPassed: false, cancellationToken);

    public Task<ProgressRecord> MarkPassedAsync(string lessonId, CancellationToken cancellationToken = default)
        => UpdateAsync(lessonId, markPassed: true, cancellationToken);

    // One shared place for "record an attempt, and maybe a pass" so the two public
    // methods can never drift apart.
    private async Task<ProgressRecord> UpdateAsync(
        string lessonId, bool markPassed, CancellationToken cancellationToken)
    {
        await using var db = await _contextFactory.CreateDbContextAsync(cancellationToken);

        var entity = await db.ProgressRecords.FindAsync([lessonId], cancellationToken);
        if (entity is null)
        {
            entity = new ProgressRecordEntity { LessonId = lessonId };
            db.ProgressRecords.Add(entity);
        }

        var now = DateTimeOffset.UtcNow;
        entity.Attempts += 1;
        entity.LastAttemptAt = now;

        // PassedAt is set only once (first pass) so "when did I first crack this?" survives retries.
        if (markPassed)
        {
            entity.ExercisePassed = true;
            entity.PassedAt ??= now;
        }

        await db.SaveChangesAsync(cancellationToken);
        return ToRecord(entity);
    }

    private static ProgressRecord ToRecord(ProgressRecordEntity entity) => new(
        entity.LessonId,
        entity.Attempts,
        entity.ExercisePassed,
        entity.LastAttemptAt,
        entity.PassedAt);
}