using System;
using System.Threading.Tasks;

public static class Demo
{
    public static async Task RunAsync()
    {
        var progress = new ProgressWithTrace(); // not real IProgress, just for demo viz
        Trace.Log("message", "Simulating progress: 0%, 40%, 70%, 100%");
        await SimulateWorkAsync(progress);
        Trace.Log("message", "Each Report() call marshalled correctly (console = direct call).");
    }

    private static async Task SimulateWorkAsync(IProgress<int> progress)
    {
        await Task.Delay(200); progress.Report(25);
        await Task.Delay(300); progress.Report(50);
        await Task.Delay(300); progress.Report(75);
        await Task.Delay(200); progress.Report(100);
    }

    private sealed class ProgressWithTrace : IProgress<int>
    {
        public void Report(int value) => Trace.Log("work-start", $"Progress: {value}%");
    }
}
