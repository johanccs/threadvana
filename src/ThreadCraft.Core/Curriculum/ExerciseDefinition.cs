namespace ThreadCraft.Core.Curriculum;

/// <summary>
/// The "Your turn" part of a lesson. The user edits <see cref="StarterCode"/> in the browser;
/// the server compiles it together with <see cref="HarnessCode"/> and runs the harness
/// in the sandbox to check correctness (behavioral assertions, not just output text).
/// </summary>
public sealed record ExerciseDefinition
{
    /// <summary>What the learner must do (Markdown, junior-friendly).</summary>
    public required string PromptMarkdown { get; init; }

    /// <summary>
    /// Code the learner starts with. Contract: must contain
    /// "public static class Solution" with the method(s) the harness will call.
    /// </summary>
    public required string StarterCode { get; init; }

    /// <summary>
    /// Validation harness source. Contract: must contain
    /// "public static class __Harness" with "public static Task&lt;HarnessResult&gt; ValidateAsync()".
    /// HarnessResult/HarnessCheck/Trace are provided by the injected prelude (docs/lesson-schema.md).
    /// Compiled together with the user's code - never shown to the learner.
    /// </summary>
    public required string HarnessCode { get; init; }

    /// <summary>The reference solution (same shape as StarterCode, completed).</summary>
    public required string ReferenceSolution { get; init; }

    /// <summary>Ordered hints, revealed one at a time on request.</summary>
    public IReadOnlyList<string> Hints { get; init; } = [];

    /// <summary>Sandbox timeout. Deadlocks/hangs surface as a friendly timeout failure.</summary>
    public int TimeoutSeconds { get; init; } = 10;
}
