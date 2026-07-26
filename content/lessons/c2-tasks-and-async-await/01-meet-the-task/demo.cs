using System;
using System.Threading;
using System.Threading.Tasks;

public static class Demo
{
    public static async Task RunAsync()
    {
        Trace.Log("thread-start", $"Main thread {Environment.CurrentManagedThreadId} starts");

        // 1. Place the order: hand the work to a thread-pool worker.
        //    Task.Run returns INSTANTLY - what you get back is a receipt
        //    (a Task<int>), not the number. The work only just started.
        Trace.Log("pool-queued", "Order placed: fetch the number (Task.Run)");
        Task<int> receipt = Task.Run(() =>
        {
            Trace.Log("pool-dequeued", $"Pool worker {Environment.CurrentManagedThreadId} picks up the order");
            Trace.Log("work-start", "Worker is fetching the number (slow!)");
            Thread.Sleep(1000); // pretend: a slow fetch, like a web call
            Trace.Log("work-end", "Worker computed 42");
            return 42; // the number rides back inside the Task
        });

        // 2. The main thread is FREE while the pool worker works.
        Trace.Log("message", "Main keeps the receipt and stays free");
        Trace.Log("work-start", "Main does a small job of its own");
        Thread.Sleep(400);
        Trace.Log("work-end", "Main finished its small job");

        // 3. Collect the result. await = sit with the buzzer:
        //    pause here (no thread blocked) until the task delivers.
        Trace.Log("wait-start", "Main awaits the receipt (buzzer in hand)");
        int answer = await receipt;
        Trace.Log("wait-end", $"Buzzer rang! Main collected {answer}");

        Trace.Log("thread-end", "Main flow ends");
    }
}