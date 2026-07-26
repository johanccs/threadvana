using System.Text.Json;
using System.Text.Json.Serialization;

namespace ThreadCraft.Web.Services;

/// <summary>
/// Loads the junior-friendly glossary (content/glossary.json) so UI copy can
/// explain technical terms on hover. A missing file is fine — tips just render
/// as plain text.
/// </summary>
public sealed class GlossaryService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly IReadOnlyDictionary<string, string> _definitions;

    private GlossaryService(IReadOnlyDictionary<string, string> definitions)
        => _definitions = definitions;

    /// <summary>Loads the glossary file, or an empty glossary when it is missing or unreadable.</summary>
    public static GlossaryService LoadFrom(string glossaryPath)
    {
        if (!File.Exists(glossaryPath))
        {
            return new GlossaryService(new Dictionary<string, string>());
        }

        try
        {
            var entries = JsonSerializer.Deserialize<List<GlossaryEntry>>(
                File.ReadAllText(glossaryPath), JsonOptions) ?? [];

            var definitions = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var entry in entries)
            {
                if (!string.IsNullOrWhiteSpace(entry.Term) && !string.IsNullOrWhiteSpace(entry.Definition))
                {
                    definitions[entry.Term] = entry.Definition;
                }
            }

            return new GlossaryService(definitions);
        }
        catch (JsonException)
        {
            // A broken glossary must never take the app down.
            return new GlossaryService(new Dictionary<string, string>());
        }
    }

    /// <summary>The one-sentence definition for a term, or null when the term is unknown.</summary>
    public string? GetDefinition(string term)
        => _definitions.TryGetValue(term, out var definition) ? definition : null;

    private sealed class GlossaryEntry
    {
        [JsonPropertyName("term")]
        public string? Term { get; set; }

        [JsonPropertyName("definition")]
        public string? Definition { get; set; }
    }
}