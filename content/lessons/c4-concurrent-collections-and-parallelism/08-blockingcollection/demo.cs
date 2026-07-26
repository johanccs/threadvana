using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

public static class Demo
{
    public static async Task RunAsync()
    {
        var bc = new BlockingCollection<int>(3);
        var producer = Task.Run(() =>
        {
            for (var i = 0; i < 10; i++)
            {
                bc.Add(i);
                Trace.Log("work-start", $"Produced {i} (buffer: ~{bc.Count})");
                Thread.Sleep(100);
            }
            bc.CompleteAdding();
            Trace.Log("work-end", "Producer done");
        });
        var consumer = Task.Run(() =>
        {
            foreach (var item in bc.GetConsumingEnumerable())
            {
                Trace.Log("thread-start", $"Consuming {item}...");
                Thread.Sleep(400);
                Trace.Log("work-end", $"Consumed {item}");
            }
        });
        await Task.WhenAll(producer, consumer);
    }
}
