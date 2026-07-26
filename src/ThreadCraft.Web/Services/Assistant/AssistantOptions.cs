namespace ThreadCraft.Web.Services;

/// <summary>
/// Settings for the AI coach. Bound from the "Assistant" config section; the API key
/// can also come from the OPENROUTER_API_KEY environment variable (see Program.cs).
/// </summary>
public sealed class AssistantOptions
{
    public const string SectionName = "Assistant";

    /// <summary>OpenRouter API key. Empty means the coach is not configured yet.</summary>
    public string ApiKey { get; set; } = "";

    /// <summary>OpenRouter model id, e.g. "deepseek/deepseek-v4-pro".</summary>
    public string Model { get; set; } = "deepseek/deepseek-v4-pro";

    /// <summary>OpenRouter API root — the OpenAI-compatible endpoint.</summary>
    public string BaseUrl { get; set; } = "https://openrouter.ai/api/v1";

    /// <summary>Lesson theory is trimmed to this many characters before it is sent as context.</summary>
    public int MaxTheoryChars { get; set; } = 6000;

    /// <summary>How many recent question/answer pairs are re-sent so follow-ups make sense.</summary>
    public int MaxHistoryTurns { get; set; } = 6;

    /// <summary>Upper bound on the answer length the model may produce.</summary>
    public int MaxAnswerTokens { get; set; } = 2048;

    /// <summary>True once an API key is available — until then the UI shows setup help.</summary>
    public bool IsConfigured => !string.IsNullOrWhiteSpace(ApiKey);
}
