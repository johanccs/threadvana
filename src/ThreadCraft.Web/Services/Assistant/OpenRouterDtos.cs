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
    [property: JsonPropertyName("max_tokens")] int MaxTokens);

internal sealed class ChatResponseDto
{
    [JsonPropertyName("choices")] public ChatChoiceDto[]? Choices { get; set; }
}

internal sealed class ChatChoiceDto
{
    [JsonPropertyName("message")] public ChatMessageDto? Message { get; set; }
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
