namespace ThreadCraft.Core.Curriculum;

/// <summary>
/// A top-level course category (e.g. "Threading Foundations").
/// Categories are defined in content/lessons/categories.json.
/// </summary>
public sealed record CategoryDefinition
{
    /// <summary>Stable unique id, e.g. "c1-threading-foundations".</summary>
    public required string Id { get; init; }

    /// <summary>Display order (1-based).</summary>
    public required int Order { get; init; }

    public required string Title { get; init; }

    /// <summary>Junior-friendly one-paragraph description (Markdown allowed).</summary>
    public required string Description { get; init; }
}
