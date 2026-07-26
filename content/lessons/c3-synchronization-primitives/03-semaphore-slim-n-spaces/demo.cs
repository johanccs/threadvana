using System;
using System.Threading;
using System.Threading.Tasks;

public static class Demo
{
    // The parking lot: AT MOST 2 cars inside at any moment.
    private static readonly SemaphoreSlim _lot = new SemaphoreSlim(2);

    public static async Task RunAsync()
    {
        Trace.Log("message", "The parking lot has 2 spaces. 5 cars want in.");

        // Start all five cars at once - each is a task trying to park.
        var cars = new Task[5];
        for (int i = 0; i < 5; i++)
            cars[i] = ParkCarAsync("car-" + (i + 1));

        await Task.WhenAll(cars); // one buzzer for the whole set
        Trace.Log("message", "All five cars got a turn - never more than 2 inside!");
    }

    private static async Task ParkCarAsync(string car)
    {
        Trace.Log("wait-start", $"{car} queues at the entrance");

        await _lot.WaitAsync(); // drive in - or wait for a space to free up
        try
        {
            Trace.Log("wait-end", $"{car} got a space");
            Trace.Log("semaphore-enter", $"{car} parks (a space is taken)");
            await Task.Delay(400); // parked for a bit
            Trace.Log("semaphore-exit", $"{car} drives out (a space frees up)");
        }
        finally
        {
            _lot.Release(); // ALWAYS hand the space back - even on a crash
        }
    }
}