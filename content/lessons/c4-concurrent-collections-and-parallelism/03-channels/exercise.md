Build a one-item pipe.

Create a `Channel<string>` with capacity 1. The producer writes 3 messages
("hello", "world", "done") and then completes the channel. The consumer reads all
messages and stores the LAST one in `Solution.LastMessage`.

Use `Channel.CreateBounded<string>(1)`. The producer calls `channel.Writer.WriteAsync()`
and then `channel.Writer.Complete()`. The consumer calls `channel.Reader.ReadAsync()`
in a loop until `ReadAsync` throws `ChannelClosedException` (or use
`channel.Reader.WaitToReadAsync()`).

## Hints
1. `var channel = Channel.CreateBounded<string>(1);`
2. Producer: after writing, call `channel.Writer.Complete();`
3. Consumer: `while (await channel.Reader.WaitToReadAsync()) { var msg = await channel.Reader.ReadAsync(); LastMessage = msg; }`
