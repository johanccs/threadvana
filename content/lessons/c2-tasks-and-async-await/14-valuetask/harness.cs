using System.Threading.Tasks;

public static class __Harness
{
    public static async Task<HarnessResult> ValidateAsync()
    {
        var result = new HarnessResult();

        var cached = await Solution.GetGreetingAsync(cached: true);
        result.Add(
            name: "cached-returns-greeting",
            passed: cached == "Hello, ThreadCraft!",
            expected: "\"Hello, ThreadCraft!\" when cached signal is true",
            actual: $"\"{cached}\"",
            message: cached != "Hello, ThreadCraft!"
                ? "The synchronous (cached) path should return a hard-coded greeting."
                : "");

        var async = await Solution.GetGreetingAsync(cached: false);
        result.Add(
            name: "async-returns-backend-greeting",
            passed: async == "Hello from the backend!",
            expected: "\"Hello from the backend!\" when cached is false",
            actual: $"\"{async}\"",
            message: async != "Hello from the backend!"
                ? "The async path should wrap FetchGreetingAsync's result in a ValueTask<string>."
                : "");

        return result;
    }
}
