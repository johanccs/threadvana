using System.Threading.Tasks;

public static class Solution
{
    /// <summary>Fetches a greeting from the slow backend. Don't change.</summary>
    public static Task<string> FetchGreetingAsync()
    {
        return Task.Run(async () =>
        {
            await Task.Delay(100);
            return "Hello from the backend!";
        });
    }

    /// <summary>Return a greeting: immediate if cached, async otherwise.</summary>
    public static ValueTask<string> GetGreetingAsync(bool cached)
    {
        if (cached)
        {
            // TODO: return a ValueTask wrapping a static greeting
            return default;
        }
        // TODO: return a ValueTask wrapping the async fetch
        return default;
    }
}
