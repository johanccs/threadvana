Watch the race happen before your eyes.

The starter runs two threads, each incrementing `Solution.Counter` 50,000 times.
The expected total is 100,000. But because the counter is NOT protected, the
actual total is LESS â   lost increments.

Your job: just run it and observe. Write how many increments were lost into
`Solution.Lost`. The harness runs it TWICE and checks that `Lost > 0` in at least
one run (proving the race is real).

## Hints
1. You don't need to fix the race â   just record `Lost = 100000 - Counter`.
2. The harness runs `Run()` twice. Reset `Counter = 0` at the start of `Run()`.
3. This is a sneak preview. Category 3 teaches you all about locks and Interlocked!
