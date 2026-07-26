using System.Threading;
using System.Threading.Tasks;

public static class __Harness
{
    public static async Task<HarnessResult> ValidateAsync()
    {
        var result = new HarnessResult();
        Solution.WorkDone = 0;
        Solution.Start(2);
        for (var i = 0; i < 4; i++) Solution.QueueWork(() => Interlocked.Increment(ref Solution.WorkDone));
        await Task.Delay(500);
        result.Add("pool-works", Solution.WorkDone == 4, "4", $"{Solution.WorkDone}", "");
        return result;
    }
}
