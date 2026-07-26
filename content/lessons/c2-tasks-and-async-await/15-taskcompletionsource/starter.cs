using System.Threading.Tasks;

public static class Solution
{
    private static TaskCompletionSource? _tcs;

    /// <summary>Return a Task that completes when Trigger() is called.</summary>
    public static Task WaitForSignalAsync()
    {
        // TODO: create a TaskCompletionSource and return its Task
        return Task.CompletedTask; // placeholder
    }

    /// <summary>Complete the waiting task.</summary>
    public static void Trigger()
    {
        // TODO: call TrySetResult on the stored TCS
    }
}
