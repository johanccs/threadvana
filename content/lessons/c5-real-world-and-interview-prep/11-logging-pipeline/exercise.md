Write `Solution.WriteLogAsync(string message)` that writes to a bounded `Channel<string>(100)`. A background `Task.Run` loop drains the Channel and increments `Solution.Logged`. Return `"done"`.

## Hints
1. Use `Channel.CreateBounded<string>(100)` and `WriteAsync`.
2. Background loop: `await foreach (var msg in channel.Reader.ReadAllAsync())` + `Interlocked.Increment(ref Logged)`.
