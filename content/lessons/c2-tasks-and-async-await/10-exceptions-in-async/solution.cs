using System;
using System.Threading.Tasks;

public static class Solution
{
    public static Task<string> FetchWithErrorAsync(string id)
    {
        return Task.Run(() =>
        {
            if (id == "b") throw new InvalidOperationException($"Failed to fetch {id}");
            return $"data-{id}";
        });
    }

    public static async Task<string> TryFetchAllAsync()
    {
        var t1 = Task.Run(() => FetchWithErrorAsync("a"));
        var t2 = Task.Run(() => FetchWithErrorAsync("b"));
        var t3 = Task.Run(() => FetchWithErrorAsync("c"));

        var all = Task.WhenAll(t1, t2, t3);
        try
        {
            await all;
            return "ok";
        }
        catch
        {
            return $"error:{all.Exception!.InnerExceptions.Count}";
        }
    }
}
