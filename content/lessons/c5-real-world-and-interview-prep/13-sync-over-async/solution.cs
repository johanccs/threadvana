using System.Threading.Tasks;

public static class Solution
{
    public static Task<string> FetchAsync() => Task.FromResult("fetched-data");

    public static string CallAsyncFromSync()
        => Task.Run(() => FetchAsync()).GetAwaiter().GetResult();
}
