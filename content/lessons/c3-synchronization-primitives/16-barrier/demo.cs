using System.Threading;
using System.Threading.Tasks;

public static class Demo
{
    public static async Task RunAsync()
    {
        var barrier = new Barrier(3, b => Trace.Log("message", $"Phase {b.CurrentPhaseNumber + 1} complete"));
        Trace.Log("message", "3 workers, 3 phases — barrier releases when all arrive");
        var workers = new Task[3];
        for (var i = 0; i < 3; i++)
        {
            var idx = i;
            workers[i] = Task.Run(() =>
            {
                for (var phase = 0; phase < 3; phase++)
                {
                    Trace.Log("work-start", $"W{idx} phase {phase}");
                    Thread.Sleep(50 + idx * 20);
                    barrier.SignalAndWait();
                }
            });
        }
        await Task.WhenAll(workers);
    }
}
