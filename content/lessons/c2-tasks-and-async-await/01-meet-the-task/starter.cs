using System;
using System.Threading;
using System.Threading.Tasks;

public static class Solution
{
    // The checker reads this after awaiting RunAsync().
    // Your job: store FetchNumber()'s number here.
    public static int Result = 0;

    // PROVIDED - pretends to fetch a number slowly (like a web call).
    // Do not change this method.
    public static async Task<int> FetchNumber()
    {
        await Task.Delay(300); // pretend: slow fetch
        return 42;
    }

    public static async Task RunAsync()
    {
        // TODO: replace the placeholder line below with your own code.
        //   1. Task<int> receipt = Task.Run(() => FetchNumber());  // order placed
        //   2. int number = await receipt;                         // buzzer rang
        //   3. Result = number;                                    // collected!
        await Task.CompletedTask; // placeholder so the file compiles - delete it
    }
}