using System.Threading.Tasks;

public static class Solution
{
    /// <summary>Reference: the await way of chaining.</summary>
    public static async Task<int> ChainWithAwaitAsync()
    {
        var result = await Task.Run(() => 10);
        return result * 2;
    }

    /// <summary>Same result using ContinueWith — no async keyword.</summary>
    public static Task<int> ChainWithContinueWithAsync()
    {
        // TODO: Task.Run(() => 10).ContinueWith(...)
        return Task.FromResult(0);
    }
}
