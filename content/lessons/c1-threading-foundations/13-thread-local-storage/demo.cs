using System;
using System.Threading;
using System.Threading.Tasks;

public static class Demo
{
    [ThreadStatic]
    private static int _privateCounter;

    public static async Task RunAsync()
    {
        var t1 = new Thread(() =>
        {
            Trace.Log("thread-start", "Thread A");
            _privateCounter = 42;
            Trace.Log("message", $"Thread A set its private counter to {_privateCounter}");
            Thread.Sleep(200);
            Trace.Log("message", $"Thread A still sees {_privateCounter} (its own)");
            Trace.Log("thread-end", "Thread A");
        });
        t1.Name = "thread-a";

        var t2 = new Thread(() =>
        {
            Trace.Log("thread-start", "Thread B");
            _privateCounter = 77;
            Trace.Log("message", $"Thread B set its private counter to {_privateCounter}");
            Thread.Sleep(200);
            Trace.Log("message", $"Thread B still sees {_privateCounter} (its own — NOT 42)");
            Trace.Log("thread-end", "Thread B");
        });
        t2.Name = "thread-b";

        t1.Start();
        t2.Start();

        t1.Join();
        t2.Join();

        Trace.Log("message", $"Main thread's private counter is {_privateCounter} (default 0)");
        await Task.CompletedTask;
    }
}
