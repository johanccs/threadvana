using System.Threading.Tasks;

public static class __Harness
{
    public static async Task<HarnessResult> ValidateAsync()
    {
        var result = new HarnessResult();
        Solution.Value = 2;
        await Solution.AtomicMultiply(3);
        result.Add("cas-works", Solution.Value == 6, "6 (2×3)", $"{Solution.Value}",
            Solution.Value != 6 ? "Use CompareExchange in a CAS loop." : "");
        return result;
    }
}
