using ThreadCraft.Core.Curriculum;

namespace ThreadCraft.Core.Validation;

/// <summary>
/// The two-stage "check my code" pipeline (implemented by ThreadCraft.Execution).
/// Stage 1: Roslyn compile of (user code + harness + prelude) -> diagnostics as CompileIssues.
/// Stage 2: run in the ThreadCraft.Sandbox process with a timeout -> behavioral checks.
/// </summary>
public interface IExerciseValidator
{
    Task<ValidationResult> ValidateAsync(
        string userCode,
        ExerciseDefinition exercise,
        CancellationToken cancellationToken = default);
}
