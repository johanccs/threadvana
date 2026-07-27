using ThreadCraft.Web.Services;
using Xunit;

namespace ThreadCraft.Web.Tests;

/// <summary>
/// The coach's answers are model-generated text rendered as raw HTML, so a prompt-injected
/// or adversarial answer must not be able to run script in the learner's browser.
/// </summary>
public sealed class MarkdownRendererTests
{
    private readonly MarkdownRenderer _renderer = new();

    [Fact]
    public void Script_tags_are_stripped()
    {
        var html = _renderer.ToSanitizedMarkup("Hi <script>alert(1)</script> there").Value;

        Assert.DoesNotContain("<script", html, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Event_handler_attributes_are_stripped_from_svg()
    {
        var html = _renderer.ToSanitizedMarkup(
            "<svg viewBox=\"0 0 10 10\"><rect onload=\"alert(1)\" width=\"5\" height=\"5\"/></svg>").Value;

        Assert.DoesNotContain("onload", html, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("<svg", html);
    }

    [Fact]
    public void Foreign_object_and_script_inside_svg_are_removed()
    {
        var html = _renderer.ToSanitizedMarkup(
            "<svg viewBox=\"0 0 10 10\"><script>alert(1)</script><foreignObject><body onload=\"alert(1)\"></body></foreignObject></svg>").Value;

        Assert.DoesNotContain("<script", html, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("foreignObject", html, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("onload", html, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Legitimate_diagram_shapes_survive_sanitization()
    {
        const string answer =
            "Here:\n<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 10 10\">" +
            "<circle cx=\"5\" cy=\"5\" r=\"3\" fill=\"blue\"/><text x=\"1\" y=\"1\">Thread</text></svg>\nDone.";

        var html = _renderer.ToSanitizedMarkup(answer).Value;

        Assert.Contains("<svg", html);
        Assert.Contains("<circle", html);
        Assert.Contains("<text", html);
        Assert.Contains("Done.", html);
    }
}
