using System.Threading.Tasks;

public static class __Harness
{
    public static async Task<HarnessResult> ValidateAsync()
    {
        var result = new HarnessResult();
        var answer = await Solution.FetchFromCacheOrSourceAsync();
        result.Add(
            name: "returns-fresh-data",
            passed: answer == "fresh-data",
            expected: "\"fresh-data\" — cache is empty, should fall back to source",
            actual: $"\"{answer}\"",
            message: answer != "fresh-data"
                ? "QueryCacheAsync returns null (empty cache), so you should call and return QuerySourceAsync."
                : "");

        return result;
    }
}
