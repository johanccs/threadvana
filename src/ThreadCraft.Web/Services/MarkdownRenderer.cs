using Markdig;
using Microsoft.AspNetCore.Components;

namespace ThreadCraft.Web.Services;

/// <summary>
/// Turns lesson Markdown into HTML for the UI. All content is written by us,
/// so the rendered HTML is trusted and not sanitized.
/// </summary>
public sealed class MarkdownRenderer
{
    private readonly MarkdownPipeline _pipeline = new MarkdownPipelineBuilder()
        .UseAdvancedExtensions()
        .Build();

    /// <summary>Renders Markdown to a <see cref="MarkupString"/> Blazor can output raw.</summary>
    public MarkupString ToMarkup(string? markdown)
        => (MarkupString)Markdown.ToHtml(markdown ?? string.Empty, _pipeline);
}