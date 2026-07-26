using System;
using System.Threading.Tasks;

public static class Demo
{
    public static async Task RunAsync()
    {
        Trace.Log("message", "Old style: ContinueWith");
        var task1 = Task.Run(() =>
        {
            Trace.Log("work-start", "Work started");
            Task.Delay(300).Wait();
            Trace.Log("work-end", "Work done");
            return 99;
        });

        task1.ContinueWith(t =>
        {
            Trace.Log("thread-start", $"ContinueWith ran — result is {t.Result}");
        }, TaskScheduler.Default);

        await task1; // ensure the continuation fires before we move on

        Trace.Log("message", "Modern style: await (same thing, cleaner)");
        var result = await Task.Run(() =>
        {
            Trace.Log("work-start", "Modern work started");
            Task.Delay(300).Wait();
            return 77;
        });
        Trace.Log("message", $"await returned {result}");
    }
}
