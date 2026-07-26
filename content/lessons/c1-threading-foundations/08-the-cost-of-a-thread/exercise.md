How many desks fit in the office?

Every thread reserves ~1 MB of memory for its **stack** - its private desk.
Your app has a strict budget: **512 MB** of stack memory, no more.

Provided for you:

- `Solution.EstimateStackMemoryMb(int threadCount)` - estimates the total
  stack cost for any number of threads.
- `Solution.SpawnWorkers()` - the code that will start `Solution.ThreadCount`
  threads when the app runs. (The checker validates your number; it does not
  run the spawning.)

Your task:

1. Set `Solution.ThreadCount` to the LARGEST number of threads whose stack
   estimate stays within the 512 MB budget.
2. Do the math before you guess: the estimate for your number must be at most
   512 - and one thread more must NOT fit.

## Hints
1. Each thread costs ~1 MB, so N threads cost ~N MB. How many megabytes are in the budget?
2. 512 MB budget, 1 MB per thread - the answer is not a trick. Check what one MORE thread would cost.
3. EstimateStackMemoryMb(513) = 513, which is over budget. EstimateStackMemoryMb(512) = 512, exactly at budget.
