using System.Net;
using System.Text;
using System.Text.Json;
using ThreadCraft.Web.Services;
using Xunit;

namespace ThreadCraft.Web.Tests;

/// <summary>
/// Verifies the OpenRouter client against a stub HTTP handler: the request shape the API
/// expects, and that every failure surfaces as a friendly <see cref="AssistantException"/>.
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

    [Fact]
    public async Task AskAsync_sends_context_then_question_and_returns_the_answer()
    {
        var (service, handler) = CreateService(HttpStatusCode.OK,
            """{"choices":[{"message":{"role":"assistant","content":"A thread is a worker."}}]}""");

        var answer = await service.AskAsync(Request());

        Assert.Equal("A thread is a worker.", answer);

        var sent = JsonDocument.Parse(handler.LastRequestBody!).RootElement;
        Assert.Equal("openai/gpt-oss-20b:free", sent.GetProperty("model").GetString());

        var messages = sent.GetProperty("messages");
        Assert.Equal("system", messages[0].GetProperty("role").GetString());
        Assert.Contains("Threads 101", messages[1].GetProperty("content").GetString());
        Assert.Equal("What is a thread?", messages[messages.GetArrayLength() - 1].GetProperty("content").GetString());
    }

    [Fact]
    public async Task Rejected_key_becomes_a_friendly_message()
    {
        var (service, _) = CreateService(HttpStatusCode.Unauthorized, """{"error":{"message":"bad key"}}""");

        var ex = await Assert.ThrowsAsync<AssistantException>(() => service.AskAsync(Request()));

        Assert.Contains("API key was rejected", ex.Message);
    }

    [Fact]
    public async Task Rate_limit_tells_the_learner_to_wait()
    {
        var (service, _) = CreateService(HttpStatusCode.TooManyRequests, "{}");

        var ex = await Assert.ThrowsAsync<AssistantException>(() => service.AskAsync(Request()));

        Assert.Contains("rate-limited", ex.Message);
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
        await Assert.ThrowsAsync<AssistantException>(() => service.AskAsync(Request()));
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
                Content = new StringContent(_body, Encoding.UTF8, "application/json"),
            };
        }
    }
}
