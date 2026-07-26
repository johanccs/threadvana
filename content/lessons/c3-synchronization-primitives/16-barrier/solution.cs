using System.Threading;
using System.Threading.Tasks;

public static class Solution
{
    public static async Task<string> RunPhasesAsync()
    {
        var barrier = new Barrier(2);
        var w1 = Task.Run(() => { for (var i = 0; i < 3; i++) { Task.Delay(50).Wait(); barrier.SignalAndWait(); } });
        var w2 = Task.Run(() => { for (var i = 0; i < 3; i++) { Task.Delay(50).Wait(); barrier.SignalAndWait(); } });
        await Task.WhenAll(w1, w2);
        return "phased";
    }
}
