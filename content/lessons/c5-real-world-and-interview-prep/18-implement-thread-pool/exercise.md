Implement `Start(int workerCount)` (start N threads draining a `BlockingCollection`) and `QueueWork(Action)`. The harness starts 2 workers, queues 4 actions that increment `WorkDone`.

## Hints
`_queue = new BlockingCollection<Action>();` + `new Thread(()=>{foreach(var a in _queue.GetConsumingEnumerable())a();})`
