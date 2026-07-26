using ThreadCraft.Core.Execution;
using ThreadCraft.Core.Validation;
using Xunit;

namespace ThreadCraft.Execution.Tests;

/// <summary>Stage 2: the sandbox runs code, checks behavior, and survives abuse.</summary>
[Collection("Sandbox")]
public sealed class PipelineTests
{
    [Fact(Timeout = 60_000)]
    public async Task Exemplar_reference_solution_passes_its_own_harness()
    {
        var exercise = TestHost.LoadExemplarExercise();

        var result = await TestHost.CreateValidator()
            .ValidateAsync(exercise.ReferenceSolution, exercise);

        Assert.Equal(ValidationStatus.Passed, result.Status);
        Assert.Equal(2, result.Checks.Count);
        Assert.All(result.Checks, c => Assert.True(c.Passed, $"{c.Name} should pass: {c.FriendlyMessage}"));
    }

    [Fact(Timeout = 60_000)]
    public async Task Exemplar_starter_fails_with_friendly_guidance()
    {
        var exercise = TestHost.LoadExemplarExercise();

        var result = await TestHost.CreateValidator()
            .ValidateAsync(exercise.StarterCode, exercise);

        Assert.Equal(ValidationStatus.TestsFailed, result.Status);
        Assert.Equal(2, result.Checks.Count);
        Assert.All(result.Checks, c =>
        {
            Assert.False(c.Passed);
            Assert.False(string.IsNullOrWhiteSpace(c.FriendlyMessage));
            Assert.False(string.IsNullOrWhiteSpace(c.Expected));
        });
    }

    [Fact(Timeout = 60_000)]
    public async Task Infinite_loop_is_reported_as_a_friendly_timeout()
    {
        var userCode = """
            public static class Solution
            {
                public static void Run()
                {
                    while (true) { } // never ends
                }
            }
            """;
        var harness = """
            using System.Threading.Tasks;

            public static class __Harness
            {
                public static Task<HarnessResult> ValidateAsync()
                {
                    Solution.Run();
                    var r = new HarnessResult();
                    r.Add("never-reached", true, "-", "-", "-");
                    return Task.FromResult(r);
                }
            }
            """;

        var started = System.Diagnostics.Stopwatch.StartNew();
        var result = await TestHost.CreateValidator().ValidateAsync(
            userCode, TestHost.MakeExercise(userCode, harness, timeoutSeconds: 2));

        Assert.Equal(ValidationStatus.RuntimeError, result.Status);
        Assert.Contains("did not finish", result.RuntimeErrorMessage, StringComparison.OrdinalIgnoreCase);
        Assert.True(started.Elapsed < TimeSpan.FromSeconds(45),
            $"timeout detection took too long: {started.Elapsed}");
    }

    [Fact(Timeout = 60_000)]
    public async Task Exceptions_in_user_code_are_runtime_errors_with_the_message()
    {
        var userCode = """
            using System;

            public static class Solution
            {
                public static void Run() => throw new InvalidOperationException("boom");
            }
            """;
        var harness = """
            using System.Threading.Tasks;

            public static class __Harness
            {
                public static Task<HarnessResult> ValidateAsync()
                {
                    Solution.Run();
                    var r = new HarnessResult();
                    r.Add("never-reached", true, "-", "-", "-");
                    return Task.FromResult(r);
                }
            }
            """;

        var result = await TestHost.CreateValidator().ValidateAsync(
            userCode, TestHost.MakeExercise(userCode, harness));

        Assert.Equal(ValidationStatus.RuntimeError, result.Status);
        Assert.Contains("boom", result.RuntimeErrorMessage);
    }

    [Fact(Timeout = 60_000)]
    public async Task Bare_ThreadState_in_user_code_does_not_collide_with_prelude_usings()
    {
        // Regression: the combined sandbox file hoists ALL usings to the top; a
        // `using System.Diagnostics` in the prelude made bare `ThreadState`
        // ambiguous (CS0104) even though Stage 1 (separate trees) passed.
        var userCode = """
            using System;
            using System.Threading;

            public static class Solution
            {
                public static ThreadState SeenState;

                public static void Run()
                {
                    var t = new Thread(() => { });
                    SeenState = t.ThreadState;
                    t.Start();
                    t.Join();
                }
            }
            """;
        var harness = """
            using System.Threading.Tasks;

            public static class __Harness
            {
                public static Task<HarnessResult> ValidateAsync()
                {
                    Solution.Run();
                    var r = new HarnessResult();
                    r.Add("ran", true, "-", "-", "-");
                    return Task.FromResult(r);
                }
            }
            """;

        var result = await TestHost.CreateValidator().ValidateAsync(
            userCode, TestHost.MakeExercise(userCode, harness));

        Assert.Equal(ValidationStatus.Passed, result.Status);
    }

    [Fact(Timeout = 60_000)]
    public async Task Demo_streams_trace_events_and_completes()
    {
        var demo = TestHost.LoadExemplarDemo();
        var events = new List<ExecutionEvent>();

        await foreach (var evt in TestHost.CreateRunner().RunAsync(
            new CodeRunRequest(demo, HarnessSource: null, TimeoutSeconds: 10)))
            events.Add(evt);

        var traces = events.Where(e => e.Kind == ExecutionEventKind.Trace).ToList();
        Assert.True(traces.Count >= 6, $"expected at least 6 trace events, got {traces.Count}");
        Assert.Contains(traces, t => t.Trace!.Kind == "thread-start");
        Assert.Contains(traces, t => t.Trace!.Kind == "wait-start");

        var completed = Assert.Single(events, e => e.Kind == ExecutionEventKind.Completed);
        Assert.NotNull(completed.Result);
        Assert.DoesNotContain(events, e => e.Kind == ExecutionEventKind.Faulted);
    }
}
