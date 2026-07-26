using ThreadCraft.Core.Curriculum;
using ThreadCraft.Execution;

namespace ThreadCraft.Execution.Tests;

/// <summary>Shared helpers: real sandbox path, validator factory, content root discovery.</summary>
public static class TestHost
{
    public static ExecutionOptions Options { get; } = new()
    {
        SandboxPath = Path.Combine(AppContext.BaseDirectory, "ThreadCraft.Sandbox.dll"),
        HostKillGraceSeconds = 3
    };

    static TestHost()
    {
        if (!File.Exists(Options.SandboxPath))
            throw new FileNotFoundException(
                "ThreadCraft.Sandbox.dll not found next to the test assembly — " +
                "the test project must reference the Sandbox project.", Options.SandboxPath);
    }

    public static RoslynExerciseValidator CreateValidator() => new(new SandboxCodeRunner(Options));

    public static SandboxCodeRunner CreateRunner() => new(Options);

    public static string FindContentRoot()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null)
        {
            var candidate = Path.Combine(dir.FullName, "content", "lessons");
            if (Directory.Exists(candidate)) return candidate;
            dir = dir.Parent;
        }
        throw new DirectoryNotFoundException("Could not locate content/lessons above the test output folder.");
    }

    /// <summary>The exemplar exercise straight from disk (lesson 01-what-is-a-thread).</summary>
    public static ExerciseDefinition LoadExemplarExercise()
    {
        var dir = Path.Combine(FindContentRoot(), "c1-threading-foundations", "01-what-is-a-thread");
        return new ExerciseDefinition
        {
            PromptMarkdown = File.ReadAllText(Path.Combine(dir, "exercise.md")),
            StarterCode = File.ReadAllText(Path.Combine(dir, "starter.cs")),
            HarnessCode = File.ReadAllText(Path.Combine(dir, "harness.cs")),
            ReferenceSolution = File.ReadAllText(Path.Combine(dir, "solution.cs"))
        };
    }

    public static string LoadExemplarDemo() =>
        File.ReadAllText(Path.Combine(
            FindContentRoot(), "c1-threading-foundations", "01-what-is-a-thread", "demo.cs"));

    /// <summary>A trivial harness that adds one always-passing check.</summary>
    public const string TrivialHarness = """
        using System.Threading.Tasks;

        public static class __Harness
        {
            public static Task<HarnessResult> ValidateAsync()
            {
                var r = new HarnessResult();
                r.Add("always-passes", true, "-", "-", "-");
                return Task.FromResult(r);
            }
        }
        """;

    public static ExerciseDefinition MakeExercise(string starter, string harness, int timeoutSeconds = 10) =>
        new()
        {
            PromptMarkdown = "test",
            StarterCode = starter,
            HarnessCode = harness,
            ReferenceSolution = starter,
            TimeoutSeconds = timeoutSeconds
        };
}
