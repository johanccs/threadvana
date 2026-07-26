Match the approach to the problem.

You are given three scenarios. For each, set the corresponding flag in `Solution`
to the correct answer. The flags are `S1`, `S2`, `S3` â   each should be one of
`"thread"`, `"pool"`, `"async"`, `"parallel"`, or `"inline"`.

1. **Background file watcher that runs for the whole app lifecycle.** (S1)
2. **500 independent API calls that each take ~200ms â   want to wait for all
   results before continuing.** (S2)
3. **A 2-second CPU-heavy image resizing that must not freeze the UI.** (S3)

## Hints
1. Long-running dedicated = "thread" (new Thread).
2. I/O = "async" (async/await + Task.WhenAll).
3. CPU-heavy offload = "pool" (Task.Run or ThreadPool).
