using ThreadCraft.Content;
using ThreadCraft.Core.Curriculum;
using ThreadCraft.Core.Execution;
using ThreadCraft.Core.Validation;
using Xunit;

namespace ThreadCraft.Execution.Tests;

/// <summary>
/// Guards the course itself: every lesson's reference solution must pass its own
/// harness, and every demo must run cleanly through the real pipeline. As content
/// grows, these tests automatically cover new lessons.
/// </summary>
[Collection("Sandbox")]
public sealed class ContentValidationTests
{
    [Fact(Timeout = 600_000)]
    public async Task Every_reference_solution_passes_its_own_harness()
    {
        var curriculum = ContentCurriculumService.LoadFrom(TestHost.FindContentRoot());
        var validator = TestHost.CreateValidator();

        var lessonsWithExercises = OnlyRequestedLessons(curriculum.GetCategories()
            .SelectMany(c => curriculum.GetLessons(c.Id))
            .Where(l => l.Exercise is not null));

        Assert.NotEmpty(lessonsWithExercises);

        foreach (var lesson in lessonsWithExercises)
        {
            var result = await validator.ValidateAsync(
                lesson.Exercise!.ReferenceSolution, lesson.Exercise);

            Assert.True(
                result.Status == ValidationStatus.Passed,
                $"Lesson {lesson.Id} reference solution should pass its harness but got " +
                $"{result.Status}: {FirstFailureDetail(result)}");
        }
    }

    [Fact(Timeout = 600_000)]
    public async Task Every_demo_runs_to_completion()
    {
        var curriculum = ContentCurriculumService.LoadFrom(TestHost.FindContentRoot());
        var runner = TestHost.CreateRunner();

        var lessonsWithDemos = OnlyRequestedLessons(curriculum.GetCategories()
            .SelectMany(c => curriculum.GetLessons(c.Id))
            .Where(l => l.DemoCode is not null));

        Assert.NotEmpty(lessonsWithDemos);

        foreach (var lesson in lessonsWithDemos)
        {
            ExecutionEvent? final = null;
            await foreach (var evt in runner.RunAsync(
                new CodeRunRequest(lesson.DemoCode!, HarnessSource: null, TimeoutSeconds: 15)))
            {
                if (evt.Kind is ExecutionEventKind.Completed or ExecutionEventKind.Faulted)
                    final = evt;
            }

            Assert.True(
                final is { Kind: ExecutionEventKind.Completed },
                $"Lesson {lesson.Id} demo should complete but got " +
                $"{final?.Kind.ToString() ?? "nothing"}: {final?.Text}");
        }
    }

    /// <summary>
    /// Set THREADCRAFT_ONLY_LESSON to an id prefix (e.g. "c2-l05" or "c2-") to validate only
    /// matching lessons — keeps the feedback loop fast while writing new course content.
    /// </summary>
    private static IReadOnlyList<LessonDefinition> OnlyRequestedLessons(IEnumerable<LessonDefinition> lessons)
    {
        var prefix = Environment.GetEnvironmentVariable("THREADCRAFT_ONLY_LESSON");
        var list = lessons.ToList();
        return string.IsNullOrWhiteSpace(prefix)
            ? list
            : list.Where(l => l.Id.StartsWith(prefix, StringComparison.Ordinal)).ToList();
    }

    private static string FirstFailureDetail(ValidationResult result) =>
        result.RuntimeErrorMessage
        ?? result.Checks.FirstOrDefault(c => !c.Passed)?.FriendlyMessage
        ?? result.CompileIssues.FirstOrDefault()?.FriendlyMessage
        ?? "no detail";
}
