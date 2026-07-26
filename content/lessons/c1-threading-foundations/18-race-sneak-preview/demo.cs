using System;
using System.Threading;
using System.Threading.Tasks;

public static class Demo
{
    private static int _sharedCounter;

    public static async Task RunAsync()
    {
        _sharedCounter = 0;

        var t1 = new Thread(() =>
        {
            Trace.Log("thread-start", "Thread A starts");
            for (var i = 0; i < 50_000; i++)
            {
                _sharedCounter++; // NOT atomic — race here!
                if (i % 20_000 == 0) Thread.Sleep(0); // encourage swapping
            }
            Trace.Log("thread-end", "Thread A done");
        });
        t1.Name = "thread-a";

        var t2 = new Thread(() =>
        {
            Trace.Log("thread-start", "Thread B starts");
            for (var i = 0; i < 50_000; i++)
            {
                _sharedCounter++;
                if (i % 20_000 == 0) Thread.Sleep(0);
            }
            Trace.Log("thread-end", "Thread B done");
        });
        t2.Name = "thread-b";

        t1.Start(); t2.Start();
        t1.Join(); t2.Join();

        Trace.Log("message", $"Final counter: {_sharedCounter} (expected 100000, lost {100000 - _sharedCounter})");
        await Task.CompletedTask;
    }
}
