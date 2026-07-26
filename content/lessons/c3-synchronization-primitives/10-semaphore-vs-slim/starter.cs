using System.Threading;
using System.Threading.Tasks;

public static class Solution
{
    public static readonly int NumWorkers = 3;

    private static readonly SemaphoreSlim _semaphore = new(NumWorkers, NumWorkers);

    /// <summary>Enter the semaphore, do work, release.</summary>
    public static async Task ProcessAsync(int id)
    {
        // TODO: WaitAsync, work, Release
    }
}
