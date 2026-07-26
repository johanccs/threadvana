Borrow, don't hire.

`Solution.ProcessOrder(int id)` is provided: it processes one order (~150ms),
records the order id and the thread that did it, then signals
`Solution.Done` - a `CountdownEvent` that starts at 3 and reaches zero when
all three orders are finished.

Inside `Solution.Run()`:

1. Hand all three orders to the thread pool:
   `ThreadPool.QueueUserWorkItem(_ => ProcessOrder(1));` - and the same for
   orders 2 and 3.
2. Wait until all three are done: `Solution.Done.Wait();`

The checker verifies that orders 1, 2 and 3 were each processed exactly
once - and that the work really ran on the pool's own workers, not on
threads you hired yourself.

## Hints
1. One QueueUserWorkItem call per order - three calls in total.
2. Pool threads cannot be Joined - that is exactly what Done.Wait() is for.
3. Made your own `new Thread`? That is precisely the habit this exercise replaces: hand the work to the pool instead.
