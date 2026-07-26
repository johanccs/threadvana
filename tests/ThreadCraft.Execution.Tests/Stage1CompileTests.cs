using ThreadCraft.Core.Validation;
using Xunit;

namespace ThreadCraft.Execution.Tests;

/// <summary>Stage 1: compile diagnostics are mapped to friendly, correctly-located issues.</summary>
[Collection("Sandbox")]
public sealed class Stage1CompileTests
{
    [Fact]
    public async Task Missing_semicolon_is_a_friendly_compile_error_with_the_right_line()
    {
        var userCode = """
            using System;

            public static class Solution
            {
                public static void Run()
                {
                    int x = 1
                }
            }
            """;

        var result = await TestHost.CreateValidator().ValidateAsync(
            userCode, TestHost.MakeExercise(userCode, TestHost.TrivialHarness));

        Assert.Equal(ValidationStatus.CompileError, result.Status);
        var issue = Assert.Single(result.CompileIssues, i => i.Severity == "error");
        Assert.Equal("CS1002", issue.Code);
        Assert.Equal(7, issue.Line); // the "int x = 1" line inside the user file
        Assert.Contains("semicolon", issue.FriendlyMessage, StringComparison.OrdinalIgnoreCase);
        Assert.False(string.IsNullOrWhiteSpace(issue.RawMessage));
    }

    [Fact]
    public async Task Unknown_type_gets_a_friendly_message()
    {
        var userCode = """
            public static class Solution
            {
                public static void Run()
                {
                    ThreadX worker = new ThreadX(); // CS0246: unknown type in a type context
                }
            }
            """;

        var result = await TestHost.CreateValidator().ValidateAsync(
            userCode, TestHost.MakeExercise(userCode, TestHost.TrivialHarness));

        Assert.Equal(ValidationStatus.CompileError, result.Status);
        var issues = result.CompileIssues.Where(i => i.Code == "CS0246").ToList();
        Assert.NotEmpty(issues); // ThreadX appears twice -> two CS0246 diagnostics
        Assert.All(issues, issue =>
        {
            Assert.Contains("unknown", issue.FriendlyMessage, StringComparison.OrdinalIgnoreCase);
            Assert.Contains("ThreadX", issue.FriendlyMessage);
        });
    }

    [Fact]
    public async Task Warnings_do_not_block_stage_2()
    {
        var userCode = """
            public static class Solution
            {
                public static void Run()
                {
                    int unused = 5; // CS0219 warning: assigned but never used
                }
            }
            """;

        var result = await TestHost.CreateValidator().ValidateAsync(
            userCode, TestHost.MakeExercise(userCode, TestHost.TrivialHarness));

        Assert.Equal(ValidationStatus.Passed, result.Status);
        Assert.Contains(result.CompileIssues, i => i.Severity == "warning" && i.Code == "CS0219");
    }
}
