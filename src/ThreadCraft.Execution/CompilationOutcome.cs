using Microsoft.CodeAnalysis;

namespace ThreadCraft.Execution;

/// <summary>Result of a Stage-1 (in-process) Roslyn compilation.</summary>
public sealed record CompilationOutcome(
    /// <summary>True when the whole submission (prelude + user + harness) emitted cleanly.</summary>
    bool Success,
    /// <summary>Errors AND warnings attributed to the user's own code file.</summary>
    IReadOnlyList<Diagnostic> UserDiagnostics,
    /// <summary>All errors, including prelude/harness (those indicate a content bug, not a learner bug).</summary>
    IReadOnlyList<Diagnostic> AllErrors);
