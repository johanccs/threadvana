using System.Threading.Tasks;

public static class __Harness
{
    public static async Task<HarnessResult> ValidateAsync()
    {
        var result = new HarnessResult();
        var answer = await Solution.FillAndDrainBag();
        result.Add(
            name: "all-four",
            passed: answer == "4",
            expected: "4 items added and taken",
            actual: answer,
            message: answer != "4" ? "Add 1..4, then Take all, return the count." : "");
        return result;
    }
}
