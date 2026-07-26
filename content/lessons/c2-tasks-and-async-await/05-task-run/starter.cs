using System.Threading;
using System.Threading.Tasks;

public static class Solution
{
    /// <summary>Called by the harness for each waiter to announce their order.</summary>
    public static void Shout(int waiterNumber)
    {
        // Pretend work — the harness supplies Done and tracks it.
        Thread.Sleep(20 + waiterNumber * 5);
        System.Console.WriteLine($"Waiter {waiterNumber} placed their order.");
        Done.Signal();
    }

    // The harness sets this up; you only call .Wait() on it at the end.
    public static CountdownEvent Done = new(0);

    /// <summary>Start three waiters — each via Task.Run — and wait for all to finish.</summary>
    public static async Task ShoutAllAsync()
    {
        // TODO: Task.Run for waiter 1, 2, 3 — then Done.Wait()
    }
}
