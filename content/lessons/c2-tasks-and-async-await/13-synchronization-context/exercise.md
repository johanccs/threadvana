Write `Solution.CheckContextAsync()` that reads `SynchronizationContext.Current` BEFORE
and AFTER an `await Task.Delay(1)` and returns a two-word string describing what it found:

- If the context is **null** both before and after, return `"none none"`.
- If it is NOT null before but null after, return `"captured lost"`.
- If it is NOT null both before and after, return `"captured captured"`.
- If it is null before but NOT null after (rare), return `"none captured"`.

In the sandbox (console), the context will always be null â   test that your method correctly
identifies `"none none"`.

## Hints
1. `SynchronizationContext.Current` is a static property in `System.Threading`.
2. The result should be lowercase with a single space â   no punctuation.
3. The exercise tests your understanding: which contexts disappear across an await and which don't.
