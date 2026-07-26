using ThreadCraft.Web.Services;
using Xunit;

namespace ThreadCraft.Web.Tests;

/// <summary>The coach is told to emit raw SVG, but models love fences — these keep diagrams visible.</summary>
public sealed class SvgFenceTests
{
    [Fact]
    public void Fenced_svg_is_unwrapped_so_the_page_renders_it()
    {
        const string answer =
            "Look here:\n```svg\n<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 10 10\"></svg>\n```\nDone.";

        var unwrapped = SvgFence.Unwrap(answer);

        Assert.DoesNotContain("```", unwrapped);
        Assert.Contains("<svg xmlns=", unwrapped);
        Assert.Contains("Done.", unwrapped);
    }

    [Fact]
    public void Html_fenced_svg_is_also_unwrapped()
    {
        const string answer = "```html\n<svg viewBox=\"0 0 10 10\"></svg>\n```";

        var unwrapped = SvgFence.Unwrap(answer);

        Assert.DoesNotContain("```", unwrapped);
        Assert.Contains("<svg viewBox=", unwrapped);
    }

    [Fact]
    public void Code_fences_stay_fenced()
    {
        const string answer = "```csharp\nvar t = new Thread(Run);\n```";

        Assert.Equal(answer, SvgFence.Unwrap(answer));
    }

    [Fact]
    public void Already_raw_svg_is_left_alone()
    {
        const string answer = "Diagram: <svg viewBox=\"0 0 10 10\"></svg> — neat.";

        Assert.Equal(answer, SvgFence.Unwrap(answer));
    }
}
