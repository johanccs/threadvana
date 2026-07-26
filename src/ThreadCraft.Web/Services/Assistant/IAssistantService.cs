namespace ThreadCraft.Web.Services;

/// <summary>
/// The AI coach behind the "Ask the coach" panel. Implementations call a hosted
/// model (OpenRouter) — the panel only sees question in, Markdown answer out.
/// </summary>
public interface IAssistantService
{
    /// <summary>False when no API key is configured; the UI then shows setup help instead.</summary>
    bool IsConfigured { get; }

    /// <summary>The model id answers come from (shown in the UI for transparency).</summary>
    string ModelName { get; }

    /// <summary>
    /// Asks the coach. Returns the answer as Markdown (may contain one raw SVG diagram).
    /// Throws <see cref="AssistantException"/> with a learner-friendly message on failure.
    /// </summary>
    Task<string> AskAsync(AssistantRequest request, CancellationToken cancellationToken = default);
}
