using System.Diagnostics;
using Microsoft.CodeAnalysis;
using ThreadCraft.Core.Curriculum;
using ThreadCraft.Core.Execution;
using ThreadCraft.Core.Validation;

namespace ThreadCraft.Execution;

/// <summary>
/// The two-stage "check my code" pipeline (docs/architecture.md).
/// Stage 1: Roslyn compile of prelude + user code + harness -> friendly compile issues.
/// Stage 2: sandboxed run of the exercise harness -> behavioral checks.
/// </summary>
public sealed class RoslynExerciseValidator : IExerciseValidator
{
    private readonly ICodeRunner _runner;
    private readonly RoslynCompilationService _compilation = new();

    public RoslynExerciseValidator(ICodeRunner runner) => _runner = runner;

    public async Task<ValidationResult> ValidateAsync(
        string userCode,
        ExerciseDefinition exercise,
        CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();

        // ---- Stage 1: does it compile? ----
        var compilation = _compilation.CompileSubmission(userCode, exercise.HarnessCode);
        var issues = compilation.UserDiagnostics
            .Select(d => d.ToCompileIssue())
            .OrderBy(i => i.Line)
            .ToList();

        if (compilation.UserDiagnostics.Any(d => d.Severity == DiagnosticSeverity.Error))
        {
            return new ValidationResult
            {
                Status = ValidationStatus.CompileError,
                CompileIssues = issues,
                Duration = stopwatch.Elapsed
            };
        }

        if (!compilation.Success)
        {
            // The user code is fine but OUR prelude/harness failed - a content bug.
            var detail = string.Join("; ", compilation.AllErrors.Select(e => $"{e.Id}: {e.GetMessage()}"));
            return new ValidationResult
            {
                Status = ValidationStatus.RuntimeError,
                RuntimeErrorMessage =
                    "This is on us: the exercise's own checking code did not compile. " +
                    $"Please report this lesson. Details: {detail}",
                Duration = stopwatch.Elapsed
            };
        }

        // ---- Stage 2: does it behave? ----
        var request = new CodeRunRequest(userCode, exercise.HarnessCode, exercise.TimeoutSeconds);
        await foreach (var evt in _runner.RunAsync(request, cancellationToken))
        {
            if (evt.Kind == ExecutionEventKind.Completed && evt.Result is not null)
            {
                // Surface non-blocking compile warnings alongside the run result.
                return evt.Result with { CompileIssues = issues };
            }

            if (evt.Kind == ExecutionEventKind.Faulted)
            {
                return new ValidationResult
                {
                    Status = ValidationStatus.RuntimeError,
                    CompileIssues = issues,
                    RuntimeErrorMessage = evt.Text ?? "The code runner failed unexpectedly.",
                    Duration = stopwatch.Elapsed
                };
            }
        }

        return new ValidationResult
        {
            Status = ValidationStatus.RuntimeError,
            CompileIssues = issues,
            RuntimeErrorMessage = "The code runner finished without producing a result.",
            Duration = stopwatch.Elapsed
        };
    }
}

