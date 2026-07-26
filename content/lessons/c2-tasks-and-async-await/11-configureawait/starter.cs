using System.Threading.Tasks;

public static class Solution
{
    /// <summary>Returns a cached value or null. Don't change.</summary>
    public static Task<string?> QueryCacheAsync() => Task.FromResult<string?>(null);

    /// <summary>Fetches from the real source. Don't change.</summary>
    public static Task<string> QuerySourceAsync() => Task.FromResult("fresh-data");

    /// <summary>Try cache first, fall back to source — both with ConfigureAwait(false).</summary>
    public static async Task<string> FetchFromCacheOrSourceAsync()
    {
        // TODO: await both with ConfigureAwait(false)
        return await Task.FromResult("stale");
    }
}
