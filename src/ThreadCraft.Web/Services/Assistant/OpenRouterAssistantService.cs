using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace ThreadCraft.Web.Services;

/// <summary>
/// Talks to OpenRouter's OpenAI-compatible chat endpoint: one POST with system prompt,
/// lesson context, recent history and the question; the answer comes back in
/// choices[0].message.content. Every failure becomes a friendly <see cref="AssistantException"/>.
/// </summary>
public sealed class OpenRouterAssistantService : IAssistantService
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly HttpClient _http;
    private readonly AssistantOptions _options;
    private readonly AssistantRateLimiter _rateLimiter;

    public OpenRouterAssistantService(HttpClient http, AssistantOptions options, AssistantRateLimiter rateLimiter)
    {
        _http = http;
        _options = options;
        _rateLimiter = rateLimiter;
    }

    public bool IsConfigured => _options.IsConfigured;

    public string ModelName => _options.Model;

    public async Task<string> AskAsync(AssistantRequest request, CancellationToken cancellationToken = default)
    {
        if (!IsConfigured)
        {
            throw new AssistantException(
                "The coach is not set up yet. Add your OpenRouter API key first (see the setup note above the chat).");
        }

        _rateLimiter.EnsureNotRateLimited();

        var messages = new List<ChatMessageDto>
        {
            new("system", AssistantPromptBuilder.BuildSystemPrompt()),
            new("user", AssistantPromptBuilder.BuildContextMessage(request, _options)),
        };

        foreach (var turn in request.History.TakeLast(_options.MaxHistoryTurns * 2))
        {
            messages.Add(new ChatMessageDto(turn.Role, turn.Content));
        }

        messages.Add(new ChatMessageDto("user", request.Question));

        var payload = new ChatRequestDto(
            _options.Model, messages.ToArray(), Temperature: 0.4, MaxTokens: _options.MaxAnswerTokens);

        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, "chat/completions")
        {
            Content = new StringContent(
                JsonSerializer.Serialize(payload, JsonOptions), Encoding.UTF8, "application/json"),
        };
        httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _options.ApiKey);
        // OpenRouter attribution headers (optional, good practice).
        httpRequest.Headers.TryAddWithoutValidation("HTTP-Referer", "https://threadcraft.local");
        httpRequest.Headers.TryAddWithoutValidation("X-Title", "ThreadCraft Academy");

        ChatResponseDto? response;
        try
        {
            using var httpResponse = await _http.SendAsync(httpRequest, cancellationToken);
            var body = await httpResponse.Content.ReadAsStringAsync(cancellationToken);

            if (!httpResponse.IsSuccessStatusCode)
            {
                throw MapFailure((int)httpResponse.StatusCode, body);
            }

            response = JsonSerializer.Deserialize<ChatResponseDto>(body, JsonOptions);
        }
        catch (AssistantException)
        {
            throw;
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw; // Learner navigated away or switched lessons — not an error.
        }
        catch (TaskCanceledException)
        {
            throw new AssistantException("The coach took too long to answer. Please try again.");
        }
        catch (HttpRequestException)
        {
            throw new AssistantException(
                "Could not reach the coach service. Check your internet connection and try again.");
        }
        catch (JsonException)
        {
            throw new AssistantException("The coach answered in a way I could not read. Please try again.");
        }

        var answer = response?.Choices?.FirstOrDefault()?.Message?.Content;
        return string.IsNullOrWhiteSpace(answer)
            ? throw new AssistantException("The coach came back with an empty answer. Please try again.")
            : answer.Trim();
    }

    private static AssistantException MapFailure(int statusCode, string body)
    {
        var detail = "";
        try
        {
            detail = JsonSerializer.Deserialize<OpenRouterErrorEnvelope>(body, JsonOptions)?.Error?.Message ?? "";
        }
        catch (JsonException)
        {
            // Body was not JSON — the status-code messages below are enough.
        }

        return statusCode switch
        {
            401 or 403 => new AssistantException(
                "The coach's API key was rejected. Check that your OpenRouter key is correct and active."),
            402 => new AssistantException(
                "The OpenRouter account is out of credits. Top it up, then ask again."),
            429 => new AssistantException(
                "The coach is rate-limited right now. Wait a few seconds and ask again."),
            >= 500 => new AssistantException(
                "The coach service is having a hiccup. Give it a moment and try again."),
            _ => new AssistantException(
                $"The coach service said no (status {statusCode})" +
                $"{(detail.Length > 0 ? $": {detail}" : "")}. Please try again."),
        };
    }
}
