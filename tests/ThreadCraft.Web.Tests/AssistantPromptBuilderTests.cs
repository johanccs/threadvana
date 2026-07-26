using ThreadCraft.Web.Services;
using Xunit;

namespace ThreadCraft.Web.Tests;

/// <summary>
/// Pins down what the coach is told: the tone it must use, the diagram format the UI
/// can render, and that the learner's lesson / code / check results make it into the prompt.
/// </summary>
public sealed class AssistantPromptBuilderTests
{
    private static AssistantRequest SampleRequest() => new()
    {
        LessonId = "c1-threading-foundations/01-what-is-a-thread",
        LessonTitle = "What is a thread?",
        CategoryTitle = "Threading Foundations",
        TheoryMarkdown = "A thread is a worker that runs one piece of code at a time.",
        ExercisePromptMarkdown = "Start two threads that print their names.",
        UserCode = "Console.WriteLine(\"hi\");",
        LastCheckSummary = "It compiled but these checks failed: names printed",
        Question = "Why use threads?",
    };

    [Fact]
    public void System_prompt_teaches_junior_tone_and_raw_svg_diagrams()
    {
        var prompt = AssistantPromptBuilder.BuildSystemPrompt();

        Assert.Contains("junior", prompt, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("Markdown", prompt);
        Assert.Contains("<svg>", prompt);
        Assert.Contains("Do NOT wrap it in a code fence", prompt);
    }

    [Fact]
    public void System_prompt_enforces_socratic_coaching()
    {
        var prompt = AssistantPromptBuilder.BuildSystemPrompt();

        Assert.Contains("guiding question", prompt, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("NEVER write the full solution", prompt);
        Assert.Contains("Escalate only with struggle", prompt);
    }

    [Fact]
    public void Context_message_carries_lesson_code_and_check_results()
    {
        var context = AssistantPromptBuilder.BuildContextMessage(SampleRequest(), new AssistantOptions());

        Assert.Contains("Threading Foundations", context);
        Assert.Contains("What is a thread?", context);
        Assert.Contains("Start two threads that print their names.", context);
        Assert.Contains("```csharp", context);
        Assert.Contains("Console.WriteLine(\"hi\");", context);
        Assert.Contains("these checks failed: names printed", context);
    }

    [Fact]
    public void Context_message_trims_overlong_theory()
    {
        var request = SampleRequest() with { TheoryMarkdown = new string('x', 10_000) };
        var options = new AssistantOptions { MaxTheoryChars = 100 };

        var context = AssistantPromptBuilder.BuildContextMessage(request, options);

        Assert.Contains("trimmed for length", context);
        Assert.DoesNotContain(new string('x', 500), context);
    }

    [Fact]
    public void Context_message_mentions_attempt_count_so_the_coach_can_calibrate()
    {
        var request = SampleRequest() with { AttemptCount = 3 };

        var context = AssistantPromptBuilder.BuildContextMessage(request, new AssistantOptions());

        Assert.Contains("3 attempts", context);
    }

    [Fact]
    public void Context_message_notes_an_empty_editor()
    {
        var request = SampleRequest() with { UserCode = "" };

        var context = AssistantPromptBuilder.BuildContextMessage(request, new AssistantOptions());

        Assert.Contains("editor is empty", context);
    }
}
