using System.Threading;
using System.Threading.Tasks;

public static class Solution
{
    public static Task<int> FetchAAsync() => Task.Run(() => { Thread.Sleep(300); return 42; });

    public static Task<int> FetchBAsync() => Task.Run(() => { Thread.Sleep(400); return 58; });

    public static async Task<int> FetchSumAsync()
    {
        var taskA = Task.Run(async () => await FetchAAsync());
        var taskB = Task.Run(async () => await FetchBAsync());

        var a = await taskA;
        var b = await taskB;
        return a + b;
    }
}
