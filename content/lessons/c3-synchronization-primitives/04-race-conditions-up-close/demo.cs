using System;
using System.Threading;
using System.Threading.Tasks;

public static class Demo
{
    private static int _counter = 0;

    public static async Task RunAsync()
    {
        const int incrementsPerWorker = 100_000;
        Trace.Log("message", "2 workers, 100,000 increments each. Expected total: 200,000.");

        var workerA = new Thread(() => Hammer("worker-A", incrementsPerWorker));
        workerA.Name = "worker-A";
        var workerB = new Thread(() => Hammer("worker-B", incrementsPerWorker));
        workerB.Name = "worker-B";

        Trace.Log("thread-start", "Main starts both workers");
        workerA.Start();
        workerB.Start();

        // Main waits for both - the damage is already done by then.
        workerA.Join();
        workerB.Join();

        int expected = incrementsPerWorker * 2;
        Trace.Log("message",
            $"Expected {expected} but got {_counter} - {expected - _counter} increments vanished!");
        Trace.Log("message", "Run the demo again: the wrong total is DIFFERENT every time.");
        Trace.Log("thread-end", "Main thread ends");
        await Task.CompletedTask;
    }

    private static void Hammer(string name, int times)
    {
        Trace.Log("thread-start", name + " starts");
        Trace.Log("work-start", name + " is hammering the shared counter");
        for (int i = 0; i < times; i++)
            _counter++; // read-add-write, unprotected - race fuel!
        Trace.Log("work-end", name + " finished its loop");
        Trace.Log("thread-end", name + " ends");
    }
}
