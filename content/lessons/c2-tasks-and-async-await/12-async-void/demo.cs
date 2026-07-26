using System.Threading;
using System.Threading.Tasks;

public static class Demo
{
    public static async Task RunAsync()
    {
        Trace.Log("message", "Three async Task workers (tracked) vs three async void workers (untracked)");

        // async Task — we can track these.
        var t1 = TrackedWorker(1);
        var t2 = TrackedWorker(2);
        var t3 = TrackedWorker(3);

        // async void — launched and immediately forgotten.
        VoidWorker(4);
        VoidWorker(5);
        VoidWorker(6);

        Trace.Log("message", "All workers fired — now waiting for the tracked ones only...");
        await Task.WhenAll(t1, t2, t3);
        Trace.Log("message", "Tracked workers are done. Void ones? Who knows — we have no way to check.");
    }

    private static async Task TrackedWorker(int id)
    {
        Trace.Log("thread-start", $"Tracked worker {id}");
        await Task.Delay(200 + id * 50);
        Trace.Log("work-end", $"Tracked worker {id} done");
    }

#pragma warning disable CS4014
    private static async void VoidWorker(int id)
    {
        Trace.Log("thread-start", $"Void worker {id}");
        await Task.Delay(300 + id * 30);
        Trace.Log("work-end", $"Void worker {id} done");
    }
#pragma warning restore CS4014
}
