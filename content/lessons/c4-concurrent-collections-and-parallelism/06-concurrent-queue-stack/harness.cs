using System.Linq;
using System.Threading.Tasks;

public static class __Harness
{
    public static async Task<HarnessResult> ValidateAsync()
    {
        var result = new HarnessResult();
        await Solution.ProcessItemsAsync();
        var stackItems = Solution.OutputStack.ToArray();
        result.Add(
            name: "items-moved",
            passed: stackItems.Length == 3 && stackItems.Contains("a"),
            expected: "All 3 items moved from queue to stack",
            actual: $"{stackItems.Length} items: [{string.Join(",", stackItems)}]",
            message: stackItems.Length != 3 ? "Dequeue all and push to OutputStack." : "");
        return result;
    }
}
