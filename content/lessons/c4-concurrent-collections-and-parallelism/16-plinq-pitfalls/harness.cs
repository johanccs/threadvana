using System.Threading.Tasks;

public static class __Harness
{
    public static async Task<HarnessResult> ValidateAsync()
    {
        var result = new HarnessResult();
        var answer = await Solution.HandleFaultedPLINQAsync();
        result.Add("catches-aggregate", answer != "0", "should catch AggregateException", answer,
            answer == "0" ? "Catch AggregateException when items >5 throw." : "");
        return result;
    }
}

