using System.Text;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace ThreadCraft.Execution;

/// <summary>
/// Combines prelude + user code (+ harness) for the two pipeline stages.
/// Stage 1 compiles SEPARATE syntax trees whose file paths ("user-code", "harness",
/// "demo") let us attribute diagnostics precisely. Stage 2 (sandbox) receives ONE
/// combined file with #line markers achieving the same mapping there — and all
/// using-directives hoisted to the top (a single file forbids usings mid-file).
/// </summary>
public static class CombinedSourceBuilder
{
    public const string UserCodePath = "user-code";
    public const string HarnessPath = "harness";
    public const string DemoPath = "demo";
    public const string PreludePath = "prelude";

    private static readonly CSharpParseOptions ParseOptions = new(LanguageVersion.CSharp12);

    // A using DIRECTIVE line (never matches "using var x = ..." or "using (x) { }").
    private static readonly Regex UsingDirective = new(
        @"^\s*(global\s+)?using\s+(static\s+)?[A-Za-z_][\w.]*\s*(=\s*[A-Za-z_][\w.<>]*\s*)?;\s*(//.*)?$",
        RegexOptions.Compiled);

    /// <summary>Trees for an exercise submission: prelude + user + harness.</summary>
    public static IReadOnlyList<SyntaxTree> CreateSubmissionTrees(string userCode, string harnessCode) =>
    [
        CreateTree(Prelude.Source, PreludePath),
        CreateTree(userCode, UserCodePath),
        CreateTree(harnessCode, HarnessPath)
    ];

    /// <summary>Trees for a demo run: prelude + demo.</summary>
    public static IReadOnlyList<SyntaxTree> CreateDemoTrees(string demoCode) =>
    [
        CreateTree(Prelude.Source, PreludePath),
        CreateTree(demoCode, DemoPath)
    ];

    /// <summary>
    /// One combined file for the sandbox: every using-directive hoisted to the top
    /// (deduplicated), then each part under a #line marker so sandbox diagnostics
    /// still map to the right virtual file and line.
    /// </summary>
    public static string BuildCombinedSource(string userCode, string? harnessCode)
    {
        var userPath = harnessCode is null ? DemoPath : UserCodePath;

        var parts = new List<(string Path, string Source)>
        {
            (PreludePath, Prelude.Source),
            (userPath, userCode)
        };
        if (harnessCode is not null)
            parts.Add((HarnessPath, harnessCode));

        var usings = new SortedSet<string>(StringComparer.Ordinal);
        var bodies = new List<(string Path, string Body)>();

        foreach (var (path, source) in parts)
        {
            var bodyLines = new List<string>();
            foreach (var line in source.Replace("\r\n", "\n").Split('\n'))
            {
                if (UsingDirective.IsMatch(line))
                    usings.Add(line.Trim());
                else
                    bodyLines.Add(line);
            }
            bodies.Add((path, string.Join('\n', bodyLines).Trim()));
        }

        var sb = new StringBuilder();
        foreach (var u in usings)
            sb.AppendLine(u);
        foreach (var (path, body) in bodies)
            sb.Append("\n#line 1 \"").Append(path).Append("\"\n").AppendLine(body);

        return sb.ToString();
    }

    private static SyntaxTree CreateTree(string source, string path) =>
        CSharpSyntaxTree.ParseText(source, ParseOptions, path: path);
}

