using System;
using System.Threading;
using System.Threading.Tasks;

public static class Solution
{
    // The checker reads this after awaiting RunAsync().
    public static int Result = 0;

    // PROVIDED - pretends to fetch a number slowly (like a web call).
    public static async Task<int> FetchNumber()
    {
        await Task.Delay(300); // pretend: slow fetch
        return 42;
    }

    public static async Task RunAsync()
    {
        // 1. Place the order: hand FetchNumber() to a thread-pool worker.
        //    We get a receipt (Task<int>) back instantly.
        Task<int> receipt = Task.Run(() => FetchNumber());

        // 2. Collect: await pauses RunAsync - WITHOUT blocking a thread -
        //    until the worker delivers the number.
        int number = await receipt;

        // 3. Keep the result where the checker can find it.
        Result = number;
    }
}