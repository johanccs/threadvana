using System.Threading.Tasks;

public static class Solution
{
    private static TaskCompletionSource? _tcs;

    public static Task WaitForSignalAsync()
    {
        _tcs = new TaskCompletionSource();
        return _tcs.Task;
    }

    public static void Trigger() => _tcs?.TrySetResult();
}
