using Microsoft.CodeAnalysis;
using ThreadCraft.Core.Validation;

namespace ThreadCraft.Execution;

/// <summary>Maps Roslyn diagnostics to editor-friendly compile issues.</summary>
public static class DiagnosticMapper
{
    public static CompileIssue ToCompileIssue(this Diagnostic diagnostic)
    {
        var position = diagnostic.Location.GetLineSpan().StartLinePosition;
        return new CompileIssue(
            Line: position.Line + 1,          // Roslyn is 0-based, editors are 1-based
            Column: position.Character + 1,
            Severity: diagnostic.Severity == DiagnosticSeverity.Error ? "error" : "warning",
            Code: diagnostic.Id,
            RawMessage: diagnostic.GetMessage(),
            FriendlyMessage: FriendlyDiagnostics.Map(diagnostic));
    }
}
