using System;
using System.Threading;
using System.Threading.Tasks;

public static class Demo
{
    private static int _counter = 0;

    public static async Task RunAsync()
    {
        Trace.Log("thread-start", $"Main thread {Environment.CurrentManagedThreadId} starts");
        Trace.Log("message", "Two workers, ONE shared counter, 1000 increments each - no lock");

        var workerA = new Thread(Increment1000Times);
        workerA.Name = "worker-A";
        var workerB = new Thread(Increment1000Times);
        workerB.Name = "worker-B";

        workerA.Start();
        workerB.Start();

        Trace.Log("wait-start", "Main waits for both workers (Join)");
        workerA.Join();
        workerB.Join();
        Trace.Log("wait-end", "Both workers are done");

        // The moment of truth: 2000 was expected, but the race stole some.
        Trace.Log("message", $"Expected 2000 but got {_counter} - {2000 - _counter} increments LOST");
        Trace.Log("thread-end", "Main thread ends - run again, the wrong number changes!");

        await Task.CompletedTask; // demo has nothing to await - the threads were Joined
    }

    private static void Increment1000Times()
    {
        string name = Thread.CurrentThread.Name;
        Trace.Log("thread-start", $"{name} starts");
        Trace.Log("work-start", $"{name} incrementing 1000 times");

        for (int i = 0; i < 1000; i++)
        {
            // "_counter++" is really three steps. Here they are spread out so you
            // can SEE the race - on real machines this interleaving happens by chance.
            int temp = _counter;  // READ
            Thread.Yield();       // gives the other worker a chance to jump in
            temp = temp + 1;      // ADD
            _counter = temp;      // WRITE - if both workers read the same value, one increment is lost
        }

        Trace.Log("work-end", $"{name} done");
        Trace.Log("thread-end", $"{name} ends");
    }
}