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
    /// Asks the coach and streams the answer as it is generated — each item is a
    /// Markdown fragment (may together contain one raw SVG diagram once fully joined).
    /// Throws <see cref="AssistantException"/> with a learner-friendly message on failure;
    /// the exception may surface partway through enumeration if the connection drops.
    /// </summary>
    IAsyncEnumerable<string> AskStreamingAsync(AssistantRequest request, CancellationToken cancellationToken = default);
}
