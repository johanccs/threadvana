using ThreadCraft.Content;
using ThreadCraft.Core.Curriculum;
using Xunit;

namespace ThreadCraft.Content.Tests;

/// <summary>
/// Tests for the curriculum loader. Each test builds a throwaway content tree
/// in a temp folder so the real course content is never touched.
/// </summary>
public sealed class ContentCurriculumServiceTests : IDisposable
{
    private readonly string _root = Path.Combine(Path.GetTempPath(), "threadcraft-tests", Guid.NewGuid().ToString("N"));

    public void Dispose()
    {
        if (Directory.Exists(_root)) Directory.Delete(_root, recursive: true);
    }

    [Fact]
    public void Loads_categories_lessons_exercises_and_demos()
    {
        WriteCategories("""[{"id":"c1-threads","order":1,"title":"Threads","description":"Start here"}]""");
        WriteLesson("c1-threads", "01-first", """
            ---
            id: c1-l01-first
            category: c1-threads
            order: 1
            title: First lesson
            difficulty: beginner
            visualization: thread-timeline
            interview:
              - q: What is a thread?
                a: A worker that runs code.
            ---
            # Hello

            Theory **body**.
            """,
            withExercise: true, withDemo: true);

        var sut = ContentCurriculumService.LoadFrom(_root);

        var category = Assert.Single(sut.GetCategories());
        Assert.Equal("c1-threads", category.Id);

        var lesson = Assert.Single(sut.GetLessons("c1-threads"));
        Assert.Equal("c1-l01-first", lesson.Id);
        Assert.Equal("First lesson", lesson.Title);
        Assert.Equal(LessonDifficulty.Beginner, lesson.Difficulty);
        Assert.Equal("thread-timeline", lesson.VisualizationId);
        Assert.Contains("Theory **body**.", lesson.TheoryMarkdown);
        Assert.NotNull(lesson.DemoCode);
        Assert.NotNull(lesson.Exercise);
        Assert.Equal("// starter", lesson.Exercise!.StarterCode.Trim());
        Assert.Equal("Do the thing.", lesson.Exercise.PromptMarkdown);
        Assert.Equal(["Try X first.", "Then try Y."], lesson.Exercise.Hints);
        Assert.Single(lesson.InterviewQuestions);
        Assert.Same(lesson, sut.GetLesson("C1-L01-FIRST")); // id lookup is case-insensitive
    }

    [Fact]
    public void Next_and_previous_cross_category_boundaries()
    {
        WriteCategories("""
            [
              {"id":"c1-a","order":1,"title":"A","description":"a"},
              {"id":"c2-b","order":2,"title":"B","description":"b"}
            ]
            """);
        WriteLesson("c1-a", "01-one", LessonMd("l1", "c1-a", 1), withExercise: false, withDemo: false);
        WriteLesson("c1-a", "02-two", LessonMd("l2", "c1-a", 2), withExercise: false, withDemo: false);
        WriteLesson("c2-b", "01-three", LessonMd("l3", "c2-b", 1), withExercise: false, withDemo: false);

        var sut = ContentCurriculumService.LoadFrom(_root);

        Assert.Equal("l2", sut.GetNextLesson("l1")!.Id);
        Assert.Equal("l3", sut.GetNextLesson("l2")!.Id);   // crosses category boundary
        Assert.Null(sut.GetNextLesson("l3"));
        Assert.Equal("l2", sut.GetPreviousLesson("l3")!.Id);
        Assert.Null(sut.GetPreviousLesson("l1"));
    }

    [Fact]
    public void Partial_exercise_files_fail_fast()
    {
        WriteCategories("""[{"id":"c1-a","order":1,"title":"A","description":"a"}]""");
        var dir = WriteLesson("c1-a", "01-one", LessonMd("l1", "c1-a", 1), withExercise: false, withDemo: false);
        File.WriteAllText(Path.Combine(dir, "starter.cs"), "// starter without harness");

        var ex = Assert.Throws<ContentLoadException>(() => ContentCurriculumService.LoadFrom(_root));
        Assert.Contains("partial exercise", ex.Message);
    }

    [Fact]
    public void Duplicate_lesson_order_in_same_category_fails_fast()
    {
        WriteCategories("""[{"id":"c1-a","order":1,"title":"A","description":"a"}]""");
        WriteLesson("c1-a", "01-one", LessonMd("l1", "c1-a", 1), withExercise: false, withDemo: false);
        WriteLesson("c1-a", "02-two", LessonMd("l2", "c1-a", 1), withExercise: false, withDemo: false); // same order

        Assert.Throws<ContentLoadException>(() => ContentCurriculumService.LoadFrom(_root));
    }

    // ---------------- helpers ----------------

    private static string LessonMd(string id, string category, int order) => $"""
        ---
        id: {id}
        category: {category}
        order: {order}
        title: Title of {id}
        ---
        Body of {id}.
        """;

    private void WriteCategories(string json)
    {
        Directory.CreateDirectory(_root);
        File.WriteAllText(Path.Combine(_root, "categories.json"), json);
    }

    private string WriteLesson(string category, string folder, string lessonMd, bool withExercise, bool withDemo)
    {
        var dir = Path.Combine(_root, category, folder);
        Directory.CreateDirectory(dir);
        File.WriteAllText(Path.Combine(dir, "lesson.md"), lessonMd);
        if (withExercise)
        {
            File.WriteAllText(Path.Combine(dir, "exercise.md"),
                "Do the thing.\n\n## Hints\n1. Try X first.\n2. Then try Y.\n");
            File.WriteAllText(Path.Combine(dir, "starter.cs"), "// starter");
            File.WriteAllText(Path.Combine(dir, "harness.cs"), "// harness");
            File.WriteAllText(Path.Combine(dir, "solution.cs"), "// solution");
        }
        if (withDemo)
            File.WriteAllText(Path.Combine(dir, "demo.cs"), "// demo");
        return dir;
    }
}
