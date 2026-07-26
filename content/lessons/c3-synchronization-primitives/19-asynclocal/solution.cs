using System.Threading;
using System.Threading.Tasks;

public static class Solution
{
    public static readonly AsyncLocal<string> Context = new();

    public static async Task<string> SetAndReadContextAsync()
    {
        Context.Value = "hello";
        await Task.Yield();
        return Context.Value ?? "lost";
    }
}
