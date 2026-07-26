You are building the order-shouting system for a restaurant. Three waiters
need to shout orders into the kitchen at the same time (the kitchen can handle
them in parallel â   no need to wait on one before the next).

1. Inside `Solution.ShoutAllAsync()`, create three tasks with `Task.Run`.
   Each task should call `Solution.Shout(waiterNumber)` where waiterNumber
   is 1, 2, or 3.
2. Wait until all three are finished shouting, then return. Use the
   `Solution.Done` signal â   call `.Wait()` on it after all three tasks complete
   (the harness sets the waiter-done counting for you).

The starter gives you the `Shout` method â   you only write `ShoutAllAsync`.

## Hints
1. `Task.Run(Action)` takes a delegate â   a lambda like `() => DoSomething()` works.
2. Three separate `var t = Task.Run(...)` calls, then await the Done signal â   the harness increments Done inside Shout, so `.Wait()` releases when all three have shouted.
3. Made a `new Thread`? This exercise is specifically about using the pool â   `Task.Run` instead of `new Thread`.
