using System;
using System.Threading.Tasks;

public static class Solution
{
    /// <summary>Fetch something by id — id "b" always fails. Don't change.</summary>
    public static Task<string> FetchWithErrorAsync(string id)
    {
        return Task.Run(() =>
        {
            if (id == "b") throw new InvalidOperationException($"Failed to fetch {id}");
            return $"data-{id}";
        });
    }

    /// <summary>Try fetching a, b, c in parallel. Return "ok" or "error:N".</summary>
    public static async Task<string> TryFetchAllAsync()
    {
        // TODO: Task.Run each, WhenAll, catch, return result
        return "not implemented";
    }
}
