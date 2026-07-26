using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace ThreadCraft.Execution;

/// <summary>
/// Stage 1 of the pipeline: compiles submissions in-process with Roslyn so learners
/// get fast, friendly compile feedback before anything runs. See docs/architecture.md
/// §Compiling (TPA references; implementation assemblies - acceptable tradeoff).
/// </summary>
public sealed class RoslynCompilationService
{
    /// <summary>Compiles an exercise submission (prelude + user code + harness).</summary>
    public CompilationOutcome CompileSubmission(string userCode, string harnessCode)
    {
        var trees = CombinedSourceBuilder.CreateSubmissionTrees(userCode, harnessCode);
        return Compile(trees, CombinedSourceBuilder.UserCodePath);
    }

    /// <summary>Compiles a demo (prelude + demo code).</summary>
    public CompilationOutcome CompileDemo(string demoCode)
    {
        var trees = CombinedSourceBuilder.CreateDemoTrees(demoCode);
        return Compile(trees, CombinedSourceBuilder.DemoPath);
    }

    private static CompilationOutcome Compile(IReadOnlyList<SyntaxTree> trees, string userPath)
    {
        var compilation = CreateCompilation(trees);

        using var peStream = new MemoryStream();
        var emit = compilation.Emit(peStream);

        var userDiagnostics = emit.Diagnostics
            .Where(d => d.Location.SourceTree?.FilePath == userPath)
            .Where(d => d.Severity is DiagnosticSeverity.Error or DiagnosticSeverity.Warning)
            .ToList();

        var allErrors = emit.Diagnostics
            .Where(d => d.Severity == DiagnosticSeverity.Error)
            .ToList();

        return new CompilationOutcome(emit.Success, userDiagnostics, allErrors);
    }

    private static CSharpCompilation CreateCompilation(IReadOnlyList<SyntaxTree> trees)
    {
        var tpa = (string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")!;
        var references = tpa.Split(Path.PathSeparator)
            .Where(p => p.EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
            .Select(p => (MetadataReference)MetadataReference.CreateFromFile(p))
            .ToList();

        return CSharpCompilation.Create(
            "Submission_" + Guid.NewGuid().ToString("N"),
            trees,
            references,
            new CSharpCompilationOptions(
                OutputKind.DynamicallyLinkedLibrary,
                nullableContextOptions: NullableContextOptions.Disable));
    }
}
