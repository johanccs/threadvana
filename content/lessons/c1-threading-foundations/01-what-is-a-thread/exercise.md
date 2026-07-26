Start your very first thread.

Inside `Solution.Run()`:

1. Create a `new Thread(...)`. Inside the thread's work, store the id of the thread
   it is running on: `Solution.WorkerThreadId = Environment.CurrentManagedThreadId;`
2. Call `Start()` on your thread.
3. Call `Join()` so `Run()` only returns after your thread has finished.

We check two things: that `WorkerThreadId` got set at all, and that it is a
*different* id than the thread that called `Run()` â   proof that a new worker really
did the job.

## Hints
1. `new Thread(() => { ... })` takes a lambda â   the code between the braces is what the new worker runs.
2. `Start()` only begins the work; `Join()` is the waiting part.
3. `Environment.CurrentManagedThreadId` tells you which thread the current line is running on.
