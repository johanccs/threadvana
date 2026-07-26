Three workers, three numbers - each one must keep its own.

`Solution.Results` is an `int` array with 4 slots, all starting at `-1`.
Slots 0, 1 and 2 are for your three threads. Slot 3 is a **tripwire**: it must
stay `-1` forever. If anything writes it, a thread used a number it should
never have seen.

Inside `Solution.Run()`:

1. Loop `i` from 0 to 2. In each round, create and start a thread.
2. Thread number `i` must store its own number: `Solution.Results[i] = i;`
   inside the thread's code.
3. Join all three threads before `Run()` returns.

Sounds easy - but this is the exact home of the loop-variable trap from the
lesson. If slot 3 gets written, or numbers go missing or doubled, your threads
are all reading the same shared `i`.

## Hints
1. The thread body needs its OWN copy of the number: write `int mine = i;` inside the loop, then use `mine` everywhere in the lambda.
2. Keep the three threads in a `Thread[]` so you can Join them all after the loop.
3. If `Results[3]` changed, your threads read `i` after the loop had ended - when `i` was already 3. That is the capture bug.
