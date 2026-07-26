using System.ComponentModel.DataAnnotations;

namespace ThreadCraft.Web.Progress;

/// <summary>
/// One row in the SQLite progress database: how far the learner got with one lesson.
/// </summary>
public sealed class ProgressRecordEntity
{
    /// <summary>The lesson this row belongs to (primary key).</summary>
    [Key]
    public required string LessonId { get; set; }

    /// <summary>How many times the learner submitted the exercise.</summary>
    public int Attempts { get; set; }

    /// <summary>True once the exercise has been passed at least once.</summary>
    public bool ExercisePassed { get; set; }

    public DateTimeOffset LastAttemptAt { get; set; }

    /// <summary>When the exercise was first passed; null until then.</summary>
    public DateTimeOffset? PassedAt { get; set; }
}