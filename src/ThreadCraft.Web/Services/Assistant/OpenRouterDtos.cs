using System.Text.Json.Serialization;

namespace ThreadCraft.Web.Services;

// Wire types for OpenRouter's OpenAI-compatible chat API (kept together like the
// Validation contract types in ThreadCraft.Core).

internal sealed record ChatMessageDto(
    [property: JsonPropertyName("role")] string Role,
    [property: JsonPropertyName("content")] string Content);

internal sealed record ChatRequestDto(
    [property: JsonPropertyName("model")] string Model,
    [property: JsonPropertyName("messages")] ChatMessageDto[] Messages,
    [property: JsonPropertyName("temperature")] double Temperature,
    [property: JsonPropertyName("max_tokens")] int MaxTokens,
    [property: JsonPropertyName("stream")] bool Stream);

/// <summary>One `data: {...}` frame from OpenRouter's server-sent-event chat stream.</summary>
internal sealed class ChatStreamChunkDto
{
    [JsonPropertyName("choices")] public ChatStreamChoiceDto[]? Choices { get; set; }
}

internal sealed class ChatStreamChoiceDto
{
    [JsonPropertyName("delta")] public ChatDeltaDto? Delta { get; set; }
}

internal sealed class ChatDeltaDto
{
    [JsonPropertyName("content")] public string? Content { get; set; }
}

/// <summary>OpenRouter error bodies look like {"error":{"message":"...","code":401}}.</summary>
internal sealed class OpenRouterErrorEnvelope
{
    [JsonPropertyName("error")] public OpenRouterErrorBody? Error { get; set; }
}

internal sealed class OpenRouterErrorBody
{
    [JsonPropertyName("message")] public string? Message { get; set; }
}
