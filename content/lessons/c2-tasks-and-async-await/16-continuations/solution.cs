using System.Threading.Tasks;

public static class Solution
{
    public static async Task<int> ChainWithAwaitAsync()
    {
        var result = await Task.Run(() => 10);
        return result * 2;
    }

    public static Task<int> ChainWithContinueWithAsync()
        => Task.Run(() => 10).ContinueWith(t => t.Result * 2);
}
