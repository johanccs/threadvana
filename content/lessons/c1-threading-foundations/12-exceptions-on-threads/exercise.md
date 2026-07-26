Catch the danger before it escapes.

`DangerousCode.Run()` throws an `InvalidOperationException` every time. The starter
code starts it on a thread-pool worker WITHOUT any try/catch. The main thread never
hears about the error.

Your job:
1. Inside `Solution.Run()`, wrap the call to `DangerousCode.Run()` in a `try`/`catch`.
2. In the catch block, save `ex.Message` into `Solution.ErrorFromWorker`.
3. After the worker finishes (use a short delay or wait), check that
   `Solution.SeenByMain` is still `"all clear"` â   the main thread was unaffected.
4. Make sure `ErrorFromWorker` now contains the word "danger" (proving you caught it).

We check: ErrorFromWorker was set, SeenByMain stayed as the main thread's own value,
and the worker caught its own error without crashing.

## Hints
1. Wrap the thread-pool work inside `new Thread(() => { ... }).Start()` â   the DEMO
   shows the pattern.
2. The catch block inside the thread is:  
   `catch (Exception ex) { Solution.ErrorFromWorker = ex.Message; }`
3. Track the main thread's view: after the thread finishes, check
   `SeenByMain` â   but do NOT set it inside the worker. The starter already sets it
   to `"all clear"` from the main thread.
