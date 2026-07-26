using System.Threading.Tasks;

public static class __Harness
{
    public static async Task<HarnessResult> ValidateAsync()
    {
        var result = new HarnessResult();
        var sum = await Solution.SumStreamAsync(5);
        result.Add(
            name: "sum-of-stream",
            passed: sum == 15,
            expected: "Sum of 1..5 = 15",
            actual: $"Sum is {sum}",
            message: sum != 15
                ? "CountToAsync should yield 1,2,3,4,5 and SumStreamAsync should add them."
                : "");
        return result;
    }
}
