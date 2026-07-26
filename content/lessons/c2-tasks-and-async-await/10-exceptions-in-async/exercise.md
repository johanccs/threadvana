`Solution.TryFetchAllAsync()` should call `Solution.FetchWithErrorAsync(string id)` for three IDs
("a", "b", "c") â   each in its own `Task.Run`, then call `Task.WhenAll` to await them.

Use a `try/catch` and return a result string:
- If all succeed: return `"ok"`.
- If any fail: return `"error:N"` where N is the total number of inner exceptions across all faulted tasks
  (hint: `WhenAll` task's `.Exception?.InnerExceptions.Count`).

`FetchWithErrorAsync` is already written â   ID "b" always fails; "a" and "c" always succeed.

## Hints
1. Store the three Task.Run results in variables, then `var all = Task.WhenAll(t1, t2, t3)`.
2. `all.Exception?.InnerExceptions.Count` gives the total number of exceptions after the WhenAll completes.
3. You'll need `using System;` for exceptions, `System.Threading.Tasks` for Task.WhenAll.
