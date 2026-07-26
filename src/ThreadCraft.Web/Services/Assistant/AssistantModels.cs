namespace ThreadCraft.Web.Services;

/// <summary>One message in the running conversation ("user" or "assistant").</summary>
public sealed record ChatTurn(string Role, string Content);

/// <summary>
/// Everything the coach needs to answer well: where the learner is in the course,
/// what their code looks like, what happened when it was checked, and the question.
/// </summary>
public sealed record AssistantRequest
{
    public required string LessonId { get; init; }
    public required string LessonTitle { get; init; }
    public required string CategoryTitle { get; init; }
    public string? TheoryMarkdown { get; init; }
    public string? ExercisePromptMarkdown { get; init; }
    public string? UserCode { get; init; }
    public string? LastCheckSummary { get; init; }

    /// <summary>How many times the learner has submitted this exercise — drives hint escalation.</summary>
    public int AttemptCount { get; init; }
    public IReadOnlyList<ChatTurn> History { get; init; } = [];
    public required string Question { get; init; }
}
