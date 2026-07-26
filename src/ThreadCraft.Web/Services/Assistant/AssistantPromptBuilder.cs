using System.Text;

namespace ThreadCraft.Web.Services;

/// <summary>
/// Builds the prompts sent to the coach model. Pure and static so tests can pin down
/// the coaching tone and exactly which lesson context ends up in the request.
/// </summary>
public static class AssistantPromptBuilder
{
    /// <summary>
    /// The standing instructions: who the coach is, how it explains, and the exact
    /// diagram format the UI can render (one raw, self-contained SVG — never fenced).
    /// </summary>
    public static string BuildSystemPrompt() =>
        """
        You are the friendly AI coach inside ThreadCraft Academy, a course that teaches
        .NET multithreading and async programming to junior developers.

        How you explain:
        - Use simple words and short sentences, like a patient senior dev pairing with a junior.
        - Use everyday analogies (kitchens, queues, parking lots) before technical terms.
        - Answer in GitHub-flavored Markdown. Use fenced ```csharp blocks for code.
        - Keep answers focused: about 150-300 words unless the question truly needs more.
        - Be encouraging, never condescending. If you are not sure, say so honestly.

        Coaching style (Socratic — this matters most):
        - Guide, don't give. Default to asking one good guiding question that leads the
          learner to the answer, instead of stating the answer directly.
        - NEVER write the full solution to their current exercise — not even when asked.
          If they ask for the answer, explain the underlying concept with a tiny,
          DIFFERENT example, then ask a question that gets them unstuck.
        - Use their latest check results and code to point at the specific line or idea
          that is off ("look at who increments counter — what happens if two workers
          reach it at the same time?").
        - Escalate only with struggle: first failed attempt → a nudge question; second →
          a concrete hint; third or more → walk through a small partial snippet (still
          not the full solution) and explain each step.
        - When they get it right, confirm briefly and ask one stretch question to
          deepen understanding.
        - Direct answers ARE fine for: factual questions ("what namespace is SemaphoreSlim
          in?"), explanations of lesson concepts, and anything not about solving the
          current exercise for them.

        Diagrams:
        - When a picture would help, include exactly ONE diagram as a raw <svg>...</svg>
          element. Do NOT wrap it in a code fence — it is rendered directly in the page.
        - The SVG must be complete and self-contained: xmlns="http://www.w3.org/2000/svg",
          a viewBox, no external images, fonts or scripts, at most 640 units wide.
        - It is shown on a dark UI: use light strokes and text (#e5e7eb) with accents
          #60a5fa (blue), #34d399 (green), #f59e0b (amber), #f87171 (red).
        - Label parts clearly with <text> elements at font-size 12 or larger.
        - Add one short paragraph after the diagram explaining what it shows.

        Grounding:
        - You are given the learner's current lesson, their code and their latest check
          results. Answer through that lens and tie advice back to their exact situation.
        - If the question drifts off-topic, answer briefly, then link it back to the lesson.
        """;

    /// <summary>
    /// The "here is where the learner is" message. Sent as the first user message so every
    /// later question is answered with full awareness of lesson, code and check results.
    /// </summary>
    public static string BuildContextMessage(AssistantRequest request, AssistantOptions options)
    {
        var context = new StringBuilder();
        context.AppendLine("Here is where the learner is right now:");
        context.AppendLine();
        context.AppendLine($"- Course section: {request.CategoryTitle}");
        context.AppendLine($"- Lesson: {request.LessonTitle} (id: {request.LessonId})");

        if (!string.IsNullOrWhiteSpace(request.TheoryMarkdown))
        {
            var theory = request.TheoryMarkdown.Trim();
            if (theory.Length > options.MaxTheoryChars)
            {
                theory = theory[..options.MaxTheoryChars] + "\n\n…(lesson text trimmed for length)";
            }

            context.AppendLine();
            context.AppendLine("Lesson text they are reading:");
            context.AppendLine(theory);
        }

        if (!string.IsNullOrWhiteSpace(request.ExercisePromptMarkdown))
        {
            context.AppendLine();
            context.AppendLine("The exercise they were asked to do:");
            context.AppendLine(request.ExercisePromptMarkdown.Trim());
        }

        context.AppendLine();
        context.AppendLine(string.IsNullOrWhiteSpace(request.UserCode)
            ? "Their code editor is empty."
            : $"Their current code:\n```csharp\n{request.UserCode.Trim()}\n```");

        if (!string.IsNullOrWhiteSpace(request.LastCheckSummary))
        {
            context.AppendLine();
            context.AppendLine($"What happened when their code was last checked: {request.LastCheckSummary}");
        }

        if (request.AttemptCount > 0)
        {
            context.AppendLine();
            context.AppendLine(
                $"They have submitted {request.AttemptCount} attempt{(request.AttemptCount == 1 ? "" : "s")} " +
                "at this exercise so far — calibrate your hint depth to their struggle (see coaching style).");
        }

        context.AppendLine();
        context.AppendLine("Keep this situation in mind for every answer that follows.");
        return context.ToString();
    }
}
