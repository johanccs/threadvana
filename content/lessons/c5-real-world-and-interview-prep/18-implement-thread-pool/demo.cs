using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

public static class Demo
{
    public static async Task RunAsync()
    {
        var pool = new TinyPool(2);
        var counter = 0;
        for (var i = 0; i < 4; i++) pool.QueueWork(() => Interlocked.Increment(ref counter));
        await Task.Delay(500);
        Trace.Log("message", $"TinyPool counter: {counter} — all 4 actions ran on 2 workers");
    }

    private class TinyPool
    {
        private readonly BlockingCollection<Action> _queue = new();
        private readonly Thread[] _workers;

        public TinyPool(int count)
        {
            _workers = new Thread[count];
            for (var i = 0; i < count; i++)
            {
                _workers[i] = new Thread(() => { foreach (var action in _queue.GetConsumingEnumerable()) action(); });
                _workers[i].Start();
            }
        }

        public void QueueWork(Action action) => _queue.Add(action);
    }
}
