Write `Solution.WaitForWorkersAsync()`:

1. Create a `CountdownEvent(3)`.
2. Start 3 tasks that do some work (Task.Delay(100)) and then call `Signal()`.
3. Call `Wait()` on the CountdownEvent to block until all 3 signal.
4. Return `"done"`.

## Hints
1. `var cde = new CountdownEvent(3); Task.Run(() => { Work(); cde.Signal(); });`
2. `cde.Wait()` blocks â   do it after starting all workers.
3. Return `"done"` after Wait unblocks.
