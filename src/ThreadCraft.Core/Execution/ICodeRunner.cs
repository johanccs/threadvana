using ThreadCraft.Core.Tracing;
using ThreadCraft.Core.Validation;

namespace ThreadCraft.Core.Execution;

/// <summary>A request to run C# source in the sandbox.</summary>
/// <param name="Source">The source to run (user/demo code). The host adds harness+prelude.</param>
/// <param name="HarnessSource">
/// Optional harness source. When present, entry point is __Harness.ValidateAsync
/// and a ValidationResult is produced. When absent, entry point is Demo.RunAsync
/// (demo mode - console + trace only).
/// </param>
/// <param name="TimeoutSeconds">Hard timeout; the sandbox self-terminates and the host kills the process after a grace period.</param>
public sealed record CodeRunRequest(
    string Source,
    string? HarnessSource,
    int TimeoutSeconds = 10);

public enum ExecutionEventKind
{
    /// <summary>A line of captured console output.</summary>
    ConsoleLine = 0,

    /// <summary>A trace event for the visualizations.</summary>
    Trace = 1,

    /// <summary>Run finished. Result is set when a harness ran; otherwise console/trace only.</summary>
    Completed = 2,

    /// <summary>Infrastructure failure (compile crash inside sandbox, process died, timeout).</summary>
    Faulted = 3
}

/// <summary>One streamed event from a running sandbox process.</summary>
public sealed record ExecutionEvent(
    ExecutionEventKind Kind,
    string? Text = null,
    TraceEvent? Trace = null,
    ValidationResult? Result = null);

/// <summary>
/// Runs C# code in the isolated sandbox process and streams back
/// console lines, trace events, and the final result.
/// Implemented by ThreadCraft.Execution.SandboxCodeRunner.
/// </summary>
public interface ICodeRunner
{
    IAsyncEnumerable<ExecutionEvent> RunAsync(
        CodeRunRequest request,
        CancellationToken cancellationToken = default);
}
