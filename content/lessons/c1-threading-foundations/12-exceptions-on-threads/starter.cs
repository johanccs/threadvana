using System;
using System.Threading;

public static class Solution
{
    // The main thread's report: it never heard about an error.
    public static string SeenByMain = "all clear — main thread was unaffected";

    // Set this inside your catch block on the worker.
    public static string ErrorFromWorker = "";

    public static void Run()
    {
        // The main thread says everything is fine.
        SeenByMain = "all clear — main thread was unaffected";

        var worker = new Thread(() =>
        {
            // TODO: wrap this in try/catch.
            //       In the catch: ErrorFromWorker = ex.Message;
            DangerousCode.Run();

            // Don't modify SeenByMain inside the worker — only the main
            // thread writes it!
        });
        worker.Name = "data-worker";
        worker.Start();
        worker.Join();
    }
}

// -- Provided — do not change ----------------------------------------------

public static class DangerousCode
{
    public static void Run()
    {
        // This always blows up — the exercise is to catch it.
        throw new InvalidOperationException("danger: the worker hit a problem");
    }
}
