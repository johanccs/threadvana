using System.Threading.Tasks;

public static class Demo
{
    public static async Task RunAsync()
    {
        var asyncLock = new AsyncLock();
        Trace.Log("message", "Two tasks competing for async lock");
        var t1 = Task.Run(async () =>
        {
            using (await asyncLock.LockAsync())
            {
                Trace.Log("work-start", "Task 1 acquired lock");
                await Task.Delay(200);
                Trace.Log("work-end", "Task 1 releasing");
            }
        });
        var t2 = Task.Run(async () =>
        {
            await Task.Delay(50);
            using (await asyncLock.LockAsync())
            {
                Trace.Log("work-start", "Task 2 acquired lock");
                await Task.Delay(100);
            }
        });
        await Task.WhenAll(t1, t2);
    }

    private class AsyncLock
    {
        private readonly System.Threading.SemaphoreSlim _sem = new(1, 1);
        public async Task<System.IDisposable> LockAsync() { await _sem.WaitAsync(); return new Releaser(_sem); }
        private struct Releaser : System.IDisposable { private System.Threading.SemaphoreSlim _s; public Releaser(System.Threading.SemaphoreSlim s) => _s = s; public void Dispose() => _s.Release(); }
    }
}
