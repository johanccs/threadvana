Same two jobs - but now you make them overlap.

`Solution.JobA()` and `Solution.JobB()` are already written for you. Each one
pretends to work for 400ms and then sets its flag (`JobARan` / `JobBRan`).

Inside `Solution.Run()`:

1. Create one thread that runs `JobA` and another thread that runs `JobB`.
2. Call `Start()` on **both** threads.
3. Call `Join()` on **both** threads, so `Run()` only returns when both jobs
   are done.

We measure how long `Run()` takes on the wall clock. One-after-another would be
~800ms. Two workers side by side should land **under 700ms**.

## Hints
1. `new Thread(JobA)` works - a method name without `()` is already "the code to run".
2. Start BOTH threads first, then Join both. Joining the first before starting the second sneaks you back to one-at-a-time.
3. If your time is around 800ms, you probably called `JobA()` and `JobB()` directly inside `Run()` - hand them to threads instead.
