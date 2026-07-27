using System.Net;
using System.Text;
using System.Text.Json;
using ThreadCraft.Web.Services;
using Xunit;

namespace ThreadCraft.Web.Tests;

/// <summary>
/// Verifies the OpenRouter client against a stub HTTP handler: the request shape the API
/// expects (including stream:true), that streamed SSE tokens come out in order, and that
/// every failure surfaces as a friendly <see cref="AssistantException"/>.
/// </summary>
public sealed class OpenRouterAssistantServiceTests
{
    private static AssistantRequest Request() => new()
    {
        LessonId = "l1",
        LessonTitle = "Threads 101",
        CategoryTitle = "Foundations",
        Question = "What is a thread?",
    };

    private static string SseBody(params string[] tokens)
    {
        var sb = new StringBuilder();
        foreach (var token in tokens)
        {
            var chunk = JsonSerializer.Serialize(new { choices = new[] { new { delta = new { content = token } } } });
            sb.Append("data: ").Append(chunk).Append("\n\n");
        }
        sb.Append("data: [DONE]\n\n");
        return sb.ToString();
    }

    private static async Task<string> CollectAsync(IAsyncEnumerable<string> stream)
    {
        var sb = new StringBuilder();
        await foreach (var chunk in stream)
        {
            sb.Append(chunk);
        }
        return sb.ToString();
    }

    [Fact]
    public async Task AskStreamingAsync_sends_context_then_question_and_streams_the_answer()
    {
        var (service, handler) = CreateService(HttpStatusCode.OK, SseBody("A thread ", "is a worker."));

        var answer = await CollectAsync(service.AskStreamingAsync(Request()));

        Assert.Equal("A thread is a worker.", answer);

        var sent = JsonDocument.Parse(handler.LastRequestBody!).RootElement;
        Assert.Equal("openai/gpt-oss-20b:free", sent.GetProperty("model").GetString());
        Assert.True(sent.GetProperty("stream").GetBoolean());

        var messages = sent.GetProperty("messages");
        Assert.Equal("system", messages[0].GetProperty("role").GetString());
        Assert.Contains("Threads 101", messages[1].GetProperty("content").GetString());
        Assert.Equal("What is a thread?", messages[messages.GetArrayLength() - 1].GetProperty("content").GetString());
    }

    [Fact]
    public async Task Rejected_key_becomes_a_friendly_message()
    {
        var (service, _) = CreateService(HttpStatusCode.Unauthorized, """{"error":{"message":"bad key"}}""");

        var ex = await Assert.ThrowsAsync<AssistantException>(() => CollectAsync(service.AskStreamingAsync(Request())));

        Assert.Contains("API key was rejected", ex.Message);
    }

    [Fact]
    public async Task Rate_limit_tells_the_learner_to_wait()
    {
        var (service, _) = CreateService(HttpStatusCode.TooManyRequests, "{}");

        var ex = await Assert.ThrowsAsync<AssistantException>(() => CollectAsync(service.AskStreamingAsync(Request())));

        Assert.Contains("rate-limited", ex.Message);
    }

    [Fact]
    public async Task Empty_stream_becomes_a_friendly_message()
    {
        var (service, _) = CreateService(HttpStatusCode.OK, "data: [DONE]\n\n");

        var ex = await Assert.ThrowsAsync<AssistantException>(() => CollectAsync(service.AskStreamingAsync(Request())));

        Assert.Contains("empty answer", ex.Message);
    }

    [Fact]
    public async Task Missing_key_fails_fast_without_any_http_call()
    {
        var handler = new StubHandler(HttpStatusCode.OK, "{}");
        var options = new AssistantOptions { ApiKey = "" };
        var service = new OpenRouterAssistantService(
            new HttpClient(handler) { BaseAddress = new Uri("https://openrouter.ai/api/v1/") },
            options,
            new AssistantRateLimiter(options, new GlobalAssistantRateLimiter(options)));

        Assert.False(service.IsConfigured);
        await Assert.ThrowsAsync<AssistantException>(() => CollectAsync(service.AskStreamingAsync(Request())));
        Assert.Null(handler.LastRequestBody);
    }

    private static (OpenRouterAssistantService, StubHandler) CreateService(HttpStatusCode status, string body)
    {
        var handler = new StubHandler(status, body);
        var options = new AssistantOptions { ApiKey = "sk-or-test" };
        var service = new OpenRouterAssistantService(
            new HttpClient(handler) { BaseAddress = new Uri("https://openrouter.ai/api/v1/") },
            options,
            new AssistantRateLimiter(options, new GlobalAssistantRateLimiter(options)));
        return (service, handler);
    }

    private sealed class StubHandler : HttpMessageHandler
    {
        private readonly HttpStatusCode _status;
        private readonly string _body;

        public StubHandler(HttpStatusCode status, string body)
        {
            _status = status;
            _body = body;
        }

        public string? LastRequestBody { get; private set; }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            LastRequestBody = request.Content is null
                ? null
                : await request.Content.ReadAsStringAsync(cancellationToken);
            return new HttpResponseMessage(_status)
            {
                Content = new StringContent(_body, Encoding.UTF8, "text/event-stream"),
            };
        }
    }
}
