Trace the state machine by hand.

The exercise uses a three-step async method — `Solution.MakeToastAsync()` — that
awaits `SpreadButterAsync()` then `AddJamAsync()` then returns "toast". The
provided helper methods record the ORDER they ran in `Solution.Log`.

Your job: trace through the method in your head (or on paper) and answer these
questions by setting the corresponding fields:

1. How many times does the state machine "pause" (return before the method is
   finished)? Set `Solution.Pauses` to the correct number.
2. Which runs FIRST — the `StartToasting()` call at the top of the method, or
   `SpreadButterAsync()` starting? Set `Solution.FirstToRun` to either
   `"StartToasting"` or `"SpreadButter"`.
3. After the first await completes, what does execution do next? Set
   `Solution.AfterFirstAwait` to either `"AddJam"` or `"return 'toast'"`.

## Hints
1. Pauses = number of `await` calls. Each `await` on an incomplete task yields.
2. Async methods start synchronously! Everything BEFORE the first `await` runs
   immediately on the calling thread.
3. After the first await finishes, the state machine jumps to the line RIGHT
   AFTER that await — which is AddJamAsync.
