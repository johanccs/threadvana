using System.Threading.Tasks;

public static class __Harness
{
    public static async Task<HarnessResult> ValidateAsync()
    {
        var result = new HarnessResult();
        var answer = await Solution.GetValueAsync("test");
        result.Add("single-flight", answer == "value-test", "value-test", answer,
            answer != "value-test" ? "Use ConcurrentDictionary + Lazy<Task<T>>." : "");
        return result;
    }
}
