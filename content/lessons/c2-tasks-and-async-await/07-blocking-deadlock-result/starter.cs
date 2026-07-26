using System.Threading;
using System.Threading.Tasks;

public static class Solution
{
    /// <summary>Fetch a number slowly (simulated). Already correct — don't change.</summary>
    public static Task<int> FetchAAsync() => Task.Run(() => { Thread.Sleep(300); return 42; });

    /// <summary>Fetch another number slowly. Already correct — don't change.</summary>
    public static Task<int> FetchBAsync() => Task.Run(() => { Thread.Sleep(400); return 58; });

    /// <summary>Call both fetchers and return their sum — rewrite to use await, not .Result.</summary>
    public static int FetchSumAsync()
    {
        var taskA = Task.Run(() => FetchAAsync().Result);
        var taskB = Task.Run(() => FetchBAsync().Result);
        Task.WaitAll(taskA, taskB);
        return taskA.Result + taskB.Result;
    }
}
