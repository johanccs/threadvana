using ThreadCraft.Core.Curriculum;
using ThreadCraft.Core.Progress;

namespace ThreadCraft.Web.Services;

/// <summary>
/// A small scoped cache of the learner's progress, shared by the layout (top bar,
/// sidebar) and the pages. Pages call <see cref="RefreshAsync"/> after a pass or
/// attempt; subscribers (the layout) are notified through <see cref="Changed"/>.
/// </summary>
public sealed class ProgressSummaryService
{
    private readonly IProgressStore _progressStore;

    public ProgressSummaryService(IProgressStore progressStore)
        => _progressStore = progressStore;

    /// <summary>Raised after a refresh so the layout can re-render checkmarks and percentages.</summary>
    public event Func<Task>? Changed;

    /// <summary>Progress by lesson id. Empty until the first refresh.</summary>
    public IReadOnlyDictionary<string, ProgressRecord> RecordsByLesson { get; private set; }
        = new Dictionary<string, ProgressRecord>();

    /// <summary>True once progress has been loaded at least once.</summary>
    public bool IsLoaded { get; private set; }

    /// <summary>Reloads progress from the store and notifies subscribers.</summary>
    public async Task RefreshAsync(CancellationToken cancellationToken = default)
    {
        var records = await _progressStore.GetAllAsync(cancellationToken);
        RecordsByLesson = records.ToDictionary(record => record.LessonId);
        IsLoaded = true;

        if (Changed is not null)
        {
            await Changed.Invoke();
        }
    }

    /// <summary>True when the lesson's exercise has been passed.</summary>
    public bool IsPassed(string lessonId)
        => RecordsByLesson.TryGetValue(lessonId, out var record) && record.ExercisePassed;

    /// <summary>How many of the given lessons have been passed.</summary>
    public int PassedCount(IEnumerable<LessonDefinition> lessons)
        => lessons.Count(lesson => IsPassed(lesson.Id));
}