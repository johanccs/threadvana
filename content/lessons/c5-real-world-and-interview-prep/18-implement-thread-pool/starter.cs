using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

public static class Solution
{
    private static BlockingCollection<Action> _queue;
    private static Thread[] _workers;

    public static void Start(int workerCount)
    {
        _queue = new BlockingCollection<Action>();
        _workers = new Thread[workerCount];
        for (var i = 0; i < workerCount; i++)
        {
            _workers[i] = new Thread(() => { foreach (var action in _queue.GetConsumingEnumerable()) action(); });
            _workers[i].Start();
        }
    }

    public static void QueueWork(Action action) => _queue.Add(action);

    public static int WorkDone;
}
