using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public static class Demo
{
    private static readonly ReaderWriterLockSlim _rwl = new();

    public static async Task RunAsync()
    {
        Trace.Log("message", "4 readers + 1 writer on ReaderWriterLockSlim");
        var tasks = new Task[5];
        for (var i = 0; i < 4; i++)
        {
            var idx = i;
            tasks[idx] = Task.Run(() =>
            {
                _rwl.EnterReadLock();
                try
                {
                    Trace.Log("thread-start", $"Reader {idx} in");
                    Thread.Sleep(200);
                    Trace.Log("work-end", $"Reader {idx} out");
                }
                finally { _rwl.ExitReadLock(); }
            });
        }
        await Task.WhenAll(tasks.Take(4).ToArray());
        tasks[4] = Task.Run(() =>
        {
            _rwl.EnterWriteLock();
            try
            {
                Trace.Log("work-start", "Writer — all readers gone");
                Thread.Sleep(300);
            }
            finally { _rwl.ExitWriteLock(); }
        });
        await tasks[4];
    }
}
