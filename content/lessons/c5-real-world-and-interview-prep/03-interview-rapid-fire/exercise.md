Answer these 10 rapid-fire scenarios. Set `Solution.Q1` through `Solution.Q10` to the
correct answer letter (A, B, C, or D).

1. Two threads do `counter++` on a shared int 1000 times each. Expected: 2000. Actual:
   usually around 1980. Why? (A: lock contention; B: ++ is not atomic; C: cached copy;
   A: integer overflow)
2. A UI method calls `Task.Delay(5000).Wait()`. What happens? (A: waits 5s gracefully;
   B: freezes the UI for 5s; C: throws; D: runs async anyway)
3. You have a 100MB image to process on a quad-core CPU. Best approach? (A: inline;
   B: Task.Run; C: Parallel.For on pixel rows; D: async/await)
4. A file watcher runs for the app's entire lifetime. Best thread type? (A: ThreadPool;
   B: new Thread + IsBackground = false; C: new Thread + IsBackground = true; D: Timer)
5. `lock(obj) { Thread.Sleep(10000); }` — what is wrong? (A: wrong object type;
   B: lock should be async; C: sleeping inside a lock starves others; D: nothing)
6. `await Task.Run(() => File.ReadAllText("big.txt"))` — what is wrong? (A: nothing;
   B: not awaited; C: Task.Run for I/O wastes a pool thread; D: missing using)
7. You see a method `async void HandleClick()`. Should you worry? (A: no; B: yes, it
   cannot be awaited and exceptions crash the process)
8. Two locks, reversed order in two threads. What can happen? (A: corruption;
   B: deadlock; C: race condition; D: nothing, they serialize)
9. Which is thread-safe without any extra locks? (A: List&lt;int&gt;;
   B: Dictionary; C: ConcurrentDictionary; D: Queue)
10. `_shouldStop` is a `bool` checked in a loop from another thread. Without `volatile`
    the loop might never see the change. Why? (A: deadlock; B: compiler/CPU caching;
    C: the field is not published; D: bools are always atomic)

## Hints
1. B (++ is not atomic — read-add-write)
2. B (freezes UI, .Wait() is synchronous)
3. C (CPU-bound large data = Parallel.For)
4. B (long-running dedicated thread, foreground so app stays alive)
5. C (never sleep/hold I/O inside a lock)
6. C (Task.Run for I/O wastes pool threads)
7. B (async void = can't be awaited; exceptions crash the process)
8. B (circular wait = deadlock)
9. C (ConcurrentDictionary is thread-safe by design)
10. B (compiler/CPU may cache the field in a register)
