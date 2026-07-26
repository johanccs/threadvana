namespace ThreadCraft.Core.Validation;

public enum ValidationStatus
{
    /// <summary>Stage 1 failed: the code does not compile.</summary>
    CompileError = 0,

    /// <summary>The code compiled but crashed or timed out while running.</summary>
    RuntimeError = 1,

    /// <summary>The code ran but one or more behavioral checks failed.</summary>
    TestsFailed = 2,

    /// <summary>All checks passed.</summary>
    Passed = 3
}

/// <summary>A single compile-time problem, mapped to an editor location.</summary>
public sealed record CompileIssue(
    int Line,
    int Column,
    string Severity,
    string Code,
    string RawMessage,
    /// <summary>Junior-friendly translation of the raw compiler message.</summary>
    string FriendlyMessage);

/// <summary>One behavioral assertion evaluated by the lesson's harness.</summary>
public sealed record ValidationCheck(
    string Name,
    bool Passed,
    string Expected,
    string Actual,
    /// <summary>Junior-friendly explanation: what went wrong and the most likely cause.</summary>
    string FriendlyMessage);

/// <summary>Full outcome of validating a learner's exercise submission.</summary>
public sealed record ValidationResult
{
    public required ValidationStatus Status { get; init; }

    /// <summary>Populated when Status == CompileError.</summary>
    public IReadOnlyList<CompileIssue> CompileIssues { get; init; } = [];

    /// <summary>Populated when the harness ran (passed or failed).</summary>
    public IReadOnlyList<ValidationCheck> Checks { get; init; } = [];

    /// <summary>Everything the user's code wrote to the console (captured).</summary>
    public string ConsoleOutput { get; init; } = "";

    /// <summary>Trace events emitted during the run (feed the visualizations).</summary>
    public IReadOnlyList<Tracing.TraceEvent> TraceEvents { get; init; } = [];

    /// <summary>Set when Status == RuntimeError (exception message or "timeout").</summary>
    public string? RuntimeErrorMessage { get; init; }

    public TimeSpan Duration { get; init; }
}
