using ThreadCraft.Content;
using Xunit;

namespace ThreadCraft.Content.Tests;

/// <summary>
/// Loads the REAL course content from the repo (content/lessons) and validates its
/// structure. Content authors run this after every batch:
///   dotnet test tests\ThreadCraft.Content.Tests --filter RealContent
/// (C# correctness of demos/exercises is verified separately by the execution
/// pipeline's content-validation tests.)
/// </summary>
public sealed class RealContentTests
{
    [Fact]
    [Trait("Category", "RealContent")]
    public void Real_course_content_loads_without_errors()
    {
        var root = FindRepoContentRoot();

        var sut = ContentCurriculumService.LoadFrom(root);

        Assert.NotEmpty(sut.GetCategories());

        // Categories whose folder exists on disk must yield lessons; categories
        // without a folder yet are batches still in flight. Once every category
        // folder exists (end of Phase 1), this check is full-strength.
        var categoriesWithFolders = sut.GetCategories()
            .Where(c => Directory.Exists(Path.Combine(root, c.Id)))
            .ToList();

        Assert.NotEmpty(categoriesWithFolders);
        foreach (var category in categoriesWithFolders)
            Assert.NotEmpty(sut.GetLessons(category.Id));
    }

    [Fact]
    [Trait("Category", "RealContent")]
    public void Every_lesson_has_theory_and_every_exercise_has_hints()
    {
        var root = FindRepoContentRoot();
        var sut = ContentCurriculumService.LoadFrom(root);

        foreach (var category in sut.GetCategories())
        foreach (var lesson in sut.GetLessons(category.Id))
        {
            Assert.False(string.IsNullOrWhiteSpace(lesson.TheoryMarkdown),
                $"{lesson.Id}: theory is empty");
            Assert.False(string.IsNullOrWhiteSpace(lesson.Title),
                $"{lesson.Id}: title is empty");
            if (lesson.Exercise is not null)
            {
                Assert.NotEmpty(lesson.Exercise.Hints);
                Assert.False(string.IsNullOrWhiteSpace(lesson.Exercise.PromptMarkdown),
                    $"{lesson.Id}: exercise prompt is empty");
            }
        }
    }

    private static string FindRepoContentRoot()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null)
        {
            var candidate = Path.Combine(dir.FullName, "content", "lessons");
            if (Directory.Exists(candidate)) return candidate;
            dir = dir.Parent;
        }
        throw new DirectoryNotFoundException("Could not locate content/lessons above the test output folder.");
    }
}
