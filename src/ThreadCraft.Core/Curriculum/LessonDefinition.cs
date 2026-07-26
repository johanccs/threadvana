namespace ThreadCraft.Core.Curriculum;

public enum LessonDifficulty
{
    Beginner = 0,
    Intermediate = 1,
    Advanced = 2
}

/// <summary>
/// A single lesson: theory (Markdown), an optional runnable visual demo,
/// an optional validated exercise, and interview-prep questions.
/// Lessons are data - they live under content/lessons and are loaded by ThreadCraft.Content.
/// </summary>
public sealed record LessonDefinition
{
    /// <summary>Stable unique id, e.g. "c1-l01-what-is-a-thread".</summary>
    public required string Id { get; init; }

    /// <summary>Id of the owning category, e.g. "c1-threading-foundations".</summary>
    public required string CategoryId { get; init; }

    /// <summary>Order within the category (1-based).</summary>
    public required int Order { get; init; }

    public required string Title { get; init; }

    public required LessonDifficulty Difficulty { get; init; }

    /// <summary>
    /// Short plain-English description of what this lesson covers.
    /// Displayed in lesson listings and category pages so the learner knows what to expect.
    /// </summary>
    public required string Description { get; init; }

    /// <summary>Theory body in Markdown. Must follow docs/writing-style.md (junior-friendly).</summary>
    public required string TheoryMarkdown { get; init; }

    /// <summary>
    /// Optional visualization component id embedded in the lesson page.
    /// Known ids: "thread-timeline", "thread-pool", "semaphore".
    /// </summary>
    public string? VisualizationId { get; init; }

    /// <summary>
    /// Optional animated-explainer scene id (front matter "explainer").
    /// A step-by-step animated flowchart that teaches the concept, shown above
    /// the live trace replay. Known ids live in SceneLibrary (Web project).
    /// </summary>
    public string? ExplainerId { get; init; }

    /// <summary>
    /// Optional runnable demo source. Contract: must contain
    /// "public static class Demo" with "public static Task RunAsync()".
    /// Demos use the injected Trace helper (see docs/lesson-schema.md).
    /// </summary>
    public string? DemoCode { get; init; }

    /// <summary>Optional exercise. Null for pure-theory lessons.</summary>
    public ExerciseDefinition? Exercise { get; init; }

    /// <summary>Interview-prep questions with junior-friendly model answers.</summary>
    public IReadOnlyList<InterviewQuestion> InterviewQuestions { get; init; } = [];
}

/// <summary>An interview question plus a simple model answer (Markdown allowed).</summary>
public sealed record InterviewQuestion(string Question, string ModelAnswer);
