using System.Threading;
using System.Threading.Tasks;

public static class Solution
{
    public static int Completed;

    public static Task FetchAsync(string url) => Task.Delay(100);

    public static async Task<string> ScrapeUrlsAsync(string[] urls)
    {
        var throttle = new SemaphoreSlim(2);
        var tasks = new Task[urls.Length];
        for (var i = 0; i < urls.Length; i++)
        {
            var url = urls[i];
            tasks[i] = Task.Run(async () =>
            {
                await throttle.WaitAsync();
                try
                {
                    await FetchAsync(url);
                    Interlocked.Increment(ref Completed);
                }
                finally { throttle.Release(); }
            });
        }
        await Task.WhenAll(tasks);
        return "done";
    }
}
