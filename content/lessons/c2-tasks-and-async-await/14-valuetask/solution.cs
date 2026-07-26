using System.Threading.Tasks;

public static class Solution
{
    public static Task<string> FetchGreetingAsync()
    {
        return Task.Run(async () =>
        {
            await Task.Delay(100);
            return "Hello from the backend!";
        });
    }

    public static ValueTask<string> GetGreetingAsync(bool cached)
    {
        if (cached)
            return new ValueTask<string>("Hello, ThreadCraft!");
        return new ValueTask<string>(FetchGreetingAsync());
    }
}
