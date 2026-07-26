using System.Threading.Tasks;

public static class Solution
{
    public static Task<string?> QueryCacheAsync() => Task.FromResult<string?>(null);

    public static Task<string> QuerySourceAsync() => Task.FromResult("fresh-data");

    public static async Task<string> FetchFromCacheOrSourceAsync()
    {
        var cached = await QueryCacheAsync().ConfigureAwait(false);
        if (cached is not null) return cached;
        return await QuerySourceAsync().ConfigureAwait(false);
    }
}
