using System;
using System.Threading;

public static class Solution
{
    public static string SeenByMain = "all clear — main thread was unaffected";
    public static string ErrorFromWorker = "";

    public static void Run()
    {
        SeenByMain = "all clear — main thread was unaffected";

        var worker = new Thread(() =>
        {
            try
            {
                DangerousCode.Run();
            }
            catch (Exception ex)
            {
                // The error stays on this thread. We catch it, record it,
                // and the main thread can read it after Join().
                ErrorFromWorker = ex.Message;
            }
        });
        worker.Name = "data-worker";
        worker.Start();
        worker.Join();
    }
}

public static class DangerousCode
{
    public static void Run() =>
        throw new InvalidOperationException("danger: the worker hit a problem");
}
