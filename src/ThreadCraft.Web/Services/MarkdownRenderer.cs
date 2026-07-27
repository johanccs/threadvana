using Ganss.Xss;
using Markdig;
using Microsoft.AspNetCore.Components;

namespace ThreadCraft.Web.Services;

/// <summary>
/// Turns Markdown into HTML for the UI. Lesson Markdown is written by us and rendered
/// as-is. Model-generated answers are untrusted (the model can be tricked via prompt
/// injection into emitting script tags or event-handler attributes), so those go
/// through <see cref="ToSanitizedMarkup"/> instead.
/// </summary>
public sealed class MarkdownRenderer
{
    private readonly MarkdownPipeline _pipeline = new MarkdownPipelineBuilder()
        .UseAdvancedExtensions()
        .Build();

    private readonly HtmlSanitizer _sanitizer = CreateSanitizer();

    /// <summary>Renders trusted, first-party Markdown to a <see cref="MarkupString"/> Blazor can output raw.</summary>
    public MarkupString ToMarkup(string? markdown)
        => (MarkupString)Markdown.ToHtml(markdown ?? string.Empty, _pipeline);

    /// <summary>Renders untrusted (AI-generated) Markdown, stripping scripts and event handlers before output.</summary>
    public MarkupString ToSanitizedMarkup(string? markdown)
    {
        var html = Markdown.ToHtml(markdown ?? string.Empty, _pipeline);
        return (MarkupString)_sanitizer.Sanitize(html);
    }

    private static HtmlSanitizer CreateSanitizer()
    {
        var sanitizer = new HtmlSanitizer();

        // Allow the SVG diagrams the coach draws, but only inert shape/text elements —
        // no <script>, <foreignObject>, <a>, or animation elements that can run script.
        foreach (var tag in new[]
        {
            "svg", "g", "path", "circle", "rect", "line", "polyline", "polygon",
            "text", "tspan", "defs", "marker", "linearGradient", "radialGradient", "stop",
        })
        {
            sanitizer.AllowedTags.Add(tag);
        }

        foreach (var attr in new[]
        {
            "viewBox", "xmlns", "width", "height", "d", "cx", "cy", "r", "x", "y", "x1", "y1", "x2", "y2",
            "points", "fill", "stroke", "stroke-width", "stroke-dasharray", "stroke-linecap", "stroke-linejoin",
            "opacity", "fill-opacity", "stroke-opacity", "transform", "font-size", "font-family", "text-anchor",
            "marker-end", "marker-start", "offset", "stop-color", "stop-opacity", "gradientUnits", "id", "class",
        })
        {
            sanitizer.AllowedAttributes.Add(attr);
        }

        return sanitizer;
    }
}