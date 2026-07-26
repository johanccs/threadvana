using System.Threading.Tasks;

public static class __Harness
{
    public static async Task<HarnessResult> ValidateAsync()
    {
        var result = new HarnessResult();
        var reply = await Solution.QuizAsync();
        result.Add(
            name: "quiz-loaded",
            passed: reply == "ok",
            expected: "\"ok\"",
            actual: $"\"{reply}\"",
            message: reply != "ok" ? "This is a review-only lesson — just return \"ok\"." : "");
        return result;
    }
}

