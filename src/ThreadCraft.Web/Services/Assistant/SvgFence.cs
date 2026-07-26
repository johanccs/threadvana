using System.Text.RegularExpressions;

namespace ThreadCraft.Web.Services;

/// <summary>
/// Models sometimes wrap their diagram in a ```svg or ```html fence even when asked not to.
/// The UI renders answers as HTML, so a fenced diagram would show as code. Unwrap those
/// fences: the raw SVG inside is passed straight through to the page instead.
/// </summary>
public static partial class SvgFence
{
    /// <summary>Replaces fenced ```svg / ```html blocks that contain an SVG with the raw SVG.</summary>
    public static string Unwrap(string markdown)
    {
        if (string.IsNullOrEmpty(markdown))
        {
            return markdown;
        }

        return FencedDiagram().Replace(markdown, match =>
        {
            var inner = match.Groups[1].Value.Trim();
            return inner.StartsWith("<svg", StringComparison.OrdinalIgnoreCase)
                ? "\n" + inner + "\n"
                : match.Value;
        });
    }

    [GeneratedRegex(@"```(?:svg|html)\s*\r?\n(.*?)```", RegexOptions.Singleline | RegexOptions.IgnoreCase)]
    private static partial Regex FencedDiagram();
}
