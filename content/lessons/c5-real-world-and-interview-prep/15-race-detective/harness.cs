using System.Threading.Tasks;

public static class __Harness
{
    public static async Task<HarnessResult> ValidateAsync()
    {
        var result = new HarnessResult();
        Solution.Balance = 0;
        await Solution.Transfer(42);
        result.Add("race-fixed", Solution.Balance == 42, "42", $"{Solution.Balance}", "");
        return result;
    }
}
