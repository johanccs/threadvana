Badge your worker.

`Solution.Work()` is provided: it pretends to work for 200ms, then records the
name of the thread it ran on into `Solution.WorkerName` (and sets
`Solution.Ran = true` so the checker knows it ran at all).

Inside `Solution.Run()`:

1. Create a `new Thread(...)` that runs `Work`.
2. Give it the name `"data-worker"` - right where you create it, because a
   thread's name can only be set once.
3. `Start()` it.
4. `Join()` it, so `Run()` returns only after the work is done.

The checker compares the recorded name with `"data-worker"` - exact spelling,
all lowercase, with a hyphen.

## Hints
1. `worker.Name = "data-worker";` goes right after `var worker = new Thread(Work);`
2. `Thread.CurrentThread` inside the work IS the worker thread - `Work()` reads its badge for you.
3. If the recorded name is null, the thread ran without a badge - you started it but never set `Name`.
