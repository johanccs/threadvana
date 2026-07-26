using System.Text.Json;
using ThreadCraft.Core.Curriculum;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace ThreadCraft.Content;

/// <summary>
/// Loads the course from disk (content/lessons) once at startup and serves it from memory.
/// Layout: categories.json + &lt;categoryId&gt;/&lt;lesson-folder&gt;/ with lesson.md (required),
/// exercise set (exercise.md + starter.cs + harness.cs + solution.cs - all or none), demo.cs (optional).
/// See docs/lesson-schema.md for the full schema.
/// </summary>
public sealed class ContentCurriculumService : ICurriculumService
{
    private static readonly string[] ExerciseFiles =
        ["exercise.md", "starter.cs", "harness.cs", "solution.cs"];

    private readonly List<CategoryDefinition> _categories;
    private readonly List<LessonDefinition> _lessonsInCourseOrder;
    private readonly Dictionary<string, LessonDefinition> _lessonsById;
    private readonly Dictionary<string, List<LessonDefinition>> _lessonsByCategory;

    private ContentCurriculumService(
        List<CategoryDefinition> categories,
        List<LessonDefinition> lessonsInCourseOrder)
    {
        _categories = categories;
        _lessonsInCourseOrder = lessonsInCourseOrder;
        _lessonsById = lessonsInCourseOrder.ToDictionary(l => l.Id, StringComparer.OrdinalIgnoreCase);
        _lessonsByCategory = lessonsInCourseOrder
            .GroupBy(l => l.CategoryId, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(
                g => g.Key,
                g => (List<LessonDefinition>)[.. g.OrderBy(l => l.Order)],
                StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>Loads and validates all content. Throws <see cref="ContentLoadException"/> on any problem.</summary>
    public static ContentCurriculumService LoadFrom(string contentRoot)
    {
        if (!Directory.Exists(contentRoot))
            throw new ContentLoadException($"Content root not found: {contentRoot}");

        var categories = LoadCategories(contentRoot);
        var lessons = new List<LessonDefinition>();

        foreach (var categoryDir in Directory.GetDirectories(contentRoot))
        {
            var categoryIdFromFolder = Path.GetFileName(categoryDir);
            if (!categories.Any(c => c.Id.Equals(categoryIdFromFolder, StringComparison.OrdinalIgnoreCase)))
                throw new ContentLoadException(
                    $"Folder '{categoryDir}' does not match any category id in categories.json.");

            foreach (var lessonDir in Directory.GetDirectories(categoryDir))
                lessons.Add(LoadLesson(lessonDir, categoryIdFromFolder));
        }

        Validate(lessons);

        var ordered = lessons
            .OrderBy(l => categories.First(c => c.Id.Equals(l.CategoryId, StringComparison.OrdinalIgnoreCase)).Order)
            .ThenBy(l => l.Order)
            .ToList();

        return new ContentCurriculumService(categories, ordered);
    }

    public IReadOnlyList<CategoryDefinition> GetCategories() => _categories;

    public IReadOnlyList<LessonDefinition> GetLessons(string categoryId) =>
        _lessonsByCategory.TryGetValue(categoryId, out var list) ? list : [];

    public LessonDefinition? GetLesson(string lessonId) =>
        _lessonsById.TryGetValue(lessonId, out var lesson) ? lesson : null;

    public LessonDefinition? GetNextLesson(string lessonId) => Neighbour(lessonId, +1);

    public LessonDefinition? GetPreviousLesson(string lessonId) => Neighbour(lessonId, -1);

    private LessonDefinition? Neighbour(string lessonId, int delta)
    {
        var index = _lessonsInCourseOrder.FindIndex(l => l.Id.Equals(lessonId, StringComparison.OrdinalIgnoreCase));
        if (index < 0) return null;
        var next = index + delta;
        return next >= 0 && next < _lessonsInCourseOrder.Count ? _lessonsInCourseOrder[next] : null;
    }

    // ---------------- loading internals ----------------

    private static List<CategoryDefinition> LoadCategories(string contentRoot)
    {
        var path = Path.Combine(contentRoot, "categories.json");
        if (!File.Exists(path))
            throw new ContentLoadException($"Missing categories file: {path}");

        List<CategoryDefinition>? categories;
        try
        {
            categories = JsonSerializer.Deserialize<List<CategoryDefinition>>(
                File.ReadAllText(path),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true, ReadCommentHandling = JsonCommentHandling.Skip });
        }
        catch (JsonException ex)
        {
            throw new ContentLoadException($"categories.json is not valid JSON: {ex.Message}");
        }

        if (categories is null || categories.Count == 0)
            throw new ContentLoadException("categories.json contains no categories.");

        if (categories.Any(c => string.IsNullOrWhiteSpace(c.Id) || string.IsNullOrWhiteSpace(c.Title)))
            throw new ContentLoadException("Every category needs a non-empty id and title.");

        if (categories.Select(c => c.Id).Distinct(StringComparer.OrdinalIgnoreCase).Count() != categories.Count)
            throw new ContentLoadException("Duplicate category id in categories.json.");

        return [.. categories.OrderBy(c => c.Order)];
    }

    private static LessonDefinition LoadLesson(string lessonDir, string categoryIdFromFolder)
    {
        var lessonPath = Path.Combine(lessonDir, "lesson.md");
        if (!File.Exists(lessonPath))
            throw new ContentLoadException($"Missing lesson.md in {lessonDir}");

        var (frontMatter, body) = SplitFrontMatter(lessonPath, File.ReadAllText(lessonPath));
        var meta = ParseFrontMatter(lessonPath, frontMatter);

        if (!meta.Category.Equals(categoryIdFromFolder, StringComparison.OrdinalIgnoreCase))
            throw new ContentLoadException(
                $"{lessonPath}: front matter category '{meta.Category}' does not match folder '{categoryIdFromFolder}'.");

        var exercise = LoadExercise(lessonDir);
        var demoPath = Path.Combine(lessonDir, "demo.cs");

        return new LessonDefinition
        {
            Id = meta.Id,
            CategoryId = meta.Category,
            Order = meta.Order,
            Title = meta.Title,
            Difficulty = ParseDifficulty(lessonPath, meta.Difficulty),
            Description = meta.Description?.Trim() ?? "",
            TheoryMarkdown = body.Trim(),
            VisualizationId = string.IsNullOrWhiteSpace(meta.Visualization) ? null : meta.Visualization.Trim(),
            ExplainerId = string.IsNullOrWhiteSpace(meta.Explainer) ? null : meta.Explainer.Trim(),
            DemoCode = File.Exists(demoPath) ? File.ReadAllText(demoPath) : null,
            Exercise = exercise,
            InterviewQuestions = meta.Interview?
                .Select(i => new InterviewQuestion(i.Q ?? "", i.A ?? ""))
                .ToList() ?? []
        };
    }

    private static ExerciseDefinition? LoadExercise(string lessonDir)
    {
        var present = ExerciseFiles.ToDictionary(
            f => f,
            f => File.Exists(Path.Combine(lessonDir, f)));

        if (present.Values.All(v => !v)) return null;

        var missing = present.Where(kv => !kv.Value).Select(kv => kv.Key).ToList();
        if (missing.Count > 0)
            throw new ContentLoadException(
                $"Lesson folder {lessonDir} has a partial exercise. Missing: {string.Join(", ", missing)}. " +
                $"An exercise needs all of: {string.Join(", ", ExerciseFiles)}.");

        var (prompt, hints) = SplitHints(File.ReadAllText(Path.Combine(lessonDir, "exercise.md")));

        return new ExerciseDefinition
        {
            PromptMarkdown = prompt,
            Hints = hints,
            StarterCode = File.ReadAllText(Path.Combine(lessonDir, "starter.cs")),
            HarnessCode = File.ReadAllText(Path.Combine(lessonDir, "harness.cs")),
            ReferenceSolution = File.ReadAllText(Path.Combine(lessonDir, "solution.cs"))
        };
    }

    /// <summary>
    /// Splits exercise.md into the prompt and the hints listed under a "## Hints"
    /// heading (one hint per numbered line, e.g. "1. Try X").
    /// </summary>
    private static (string Prompt, IReadOnlyList<string> Hints) SplitHints(string exerciseMd)
    {
        var lines = exerciseMd.Replace("\r\n", "\n").Split('\n');
        var hintsIndex = Array.FindIndex(lines, l => l.TrimStart().StartsWith("## Hints", StringComparison.OrdinalIgnoreCase));
        if (hintsIndex < 0)
            return (exerciseMd.Trim(), []);

        var prompt = string.Join('\n', lines[..hintsIndex]).Trim();
        var hints = new List<string>();
        foreach (var line in lines[(hintsIndex + 1)..])
        {
            var trimmed = line.Trim();
            if (trimmed.StartsWith("## ")) break; // next section stops hint parsing
            var dotIndex = trimmed.IndexOf(". ", StringComparison.Ordinal);
            if (dotIndex > 0 && int.TryParse(trimmed[..dotIndex], out _))
                hints.Add(trimmed[(dotIndex + 2)..].Trim());
        }
        return (prompt, hints);
    }

    private static (string FrontMatter, string Body) SplitFrontMatter(string path, string text)
    {
        using var reader = new StringReader(text);
        var first = reader.ReadLine();
        if (first is null || first.Trim() != "---")
            throw new ContentLoadException($"{path}: must start with a '---' front matter block.");

        var yamlLines = new List<string>();
        string? line;
        while ((line = reader.ReadLine()) is not null)
        {
            if (line.Trim() == "---")
                return (string.Join('\n', yamlLines), reader.ReadToEnd());
            yamlLines.Add(line);
        }
        throw new ContentLoadException($"{path}: front matter block is not closed with '---'.");
    }

    private static LessonFrontMatter ParseFrontMatter(string path, string yaml)
    {
        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .IgnoreUnmatchedProperties()
            .Build();

        LessonFrontMatter meta;
        try
        {
            meta = deserializer.Deserialize<LessonFrontMatter>(yaml) ?? new LessonFrontMatter();
        }
        catch (Exception ex)
        {
            throw new ContentLoadException($"{path}: invalid YAML front matter - {ex.Message}");
        }

        if (string.IsNullOrWhiteSpace(meta.Id) || string.IsNullOrWhiteSpace(meta.Category) ||
            string.IsNullOrWhiteSpace(meta.Title) || meta.Order <= 0)
            throw new ContentLoadException(
                $"{path}: front matter needs non-empty id, category, title and order > 0.");

        return meta;
    }

    private static LessonDifficulty ParseDifficulty(string path, string? value) =>
        value?.Trim().ToLowerInvariant() switch
        {
            null or "" or "beginner" => LessonDifficulty.Beginner,
            "intermediate" => LessonDifficulty.Intermediate,
            "advanced" => LessonDifficulty.Advanced,
            var other => throw new ContentLoadException(
                $"{path}: unknown difficulty '{other}' (beginner | intermediate | advanced).")
        };

    private static void Validate(List<LessonDefinition> lessons)
    {
        var duplicateId = lessons.GroupBy(l => l.Id, StringComparer.OrdinalIgnoreCase)
            .FirstOrDefault(g => g.Count() > 1);
        if (duplicateId is not null)
            throw new ContentLoadException($"Duplicate lesson id '{duplicateId.Key}'.");

        var duplicateOrder = lessons
            .GroupBy(l => (l.CategoryId, l.Order))
            .FirstOrDefault(g => g.Count() > 1);
        if (duplicateOrder is not null)
            throw new ContentLoadException(
                $"Duplicate order {duplicateOrder.Key.Order} in category '{duplicateOrder.Key.CategoryId}'.");
    }

    // Front matter DTOs (private on purpose - the public model is LessonDefinition).
    private sealed class LessonFrontMatter
    {
        public string Id { get; set; } = "";
        public string Category { get; set; } = "";
        public int Order { get; set; }
        public string Title { get; set; } = "";
        public string? Description { get; set; }
        public string? Difficulty { get; set; }
        public string? Visualization { get; set; }
        public string? Explainer { get; set; }
        public List<InterviewEntry>? Interview { get; set; }
    }

    private sealed class InterviewEntry
    {
        public string? Q { get; set; }
        public string? A { get; set; }
    }
}

