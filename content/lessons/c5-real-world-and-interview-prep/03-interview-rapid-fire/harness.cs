using System;
using System.Threading.Tasks;

public static class __Harness
{
    private static readonly char[] Answers = { 'B','B','C','B','C','C','B','B','C','B' };

    public static async Task<HarnessResult> ValidateAsync()
    {
        await Task.Delay(10);
        Solution.Run();

        var given = new[] { Solution.Q1, Solution.Q2, Solution.Q3, Solution.Q4,
                            Solution.Q5, Solution.Q6, Solution.Q7, Solution.Q8,
                            Solution.Q9, Solution.Q10 };
        var correct = 0;
        for (var i = 0; i < 10; i++) if (given[i] == Answers[i]) correct++;

        var result = new HarnessResult();

        result.Add(
            name: "score",
            passed: correct == 10,
            expected: "10/10 correct",
            actual: $"{correct}/10",
            message: correct switch
            {
                >= 8 => $"So close! {correct}/10. Review exercise.md for the answers to {10 - correct} question(s).",
                >= 5 => $"Halfway there — {correct}/10. Read the hints in exercise.md and try again!",
                _ => $"Keep studying — {correct}/10. Every lesson in ThreadCraft Academy has the answers."
            });

        return result;
    }
}
