namespace ThreadCraft.Core.Curriculum;

/// <summary>
/// Read-only access to the loaded course content.
/// Implemented by ThreadCraft.Content.ContentCurriculumService (loads from disk at startup).
/// </summary>
public interface ICurriculumService
{
    /// <summary>All categories, ordered.</summary>
    IReadOnlyList<CategoryDefinition> GetCategories();

    /// <summary>All lessons of a category, ordered. Empty if the category id is unknown.</summary>
    IReadOnlyList<LessonDefinition> GetLessons(string categoryId);

    /// <summary>Find a lesson by id, or null.</summary>
    LessonDefinition? GetLesson(string lessonId);

    /// <summary>Next lesson in course order (across category boundaries), or null at the end.</summary>
    LessonDefinition? GetNextLesson(string lessonId);

    /// <summary>Previous lesson in course order (across category boundaries), or null at the start.</summary>
    LessonDefinition? GetPreviousLesson(string lessonId);
}
