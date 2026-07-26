using System.Threading;
using System.Threading.Tasks;

public static class Solution
{
    public static readonly int NumWorkers = 3;
    private static readonly SemaphoreSlim _semaphore = new(NumWorkers, NumWorkers);

    public static async Task ProcessAsync(int id)
    {
        await _semaphore.WaitAsync();
        try
        {
            await Task.Delay(200);
        }
        finally { _semaphore.Release(); }
    }
}
