---
id: c2-l03-whenall-many-tasks
category: c2-tasks-and-async-await
order: 3
title: Task.WhenAll ÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬Â Run Many Things at Once
difficulty: intermediate
description: "Run many tasks at once with Task.WhenAll and collect all results when every single one is done."
visualization: thread-timeline
explainer: async-state-machine
interview:
  - q: What does Task.WhenAll do?
    a: It takes a set of tasks and returns one task that completes when every one of them has completed. You start all your jobs first, then await the combined task once, so the jobs overlap instead of running one after another. Bonus point - if any task faults, WhenAll still waits for all of them and hands you every exception, not just the first.
  - q: Why is awaiting tasks one by one slower?
    a: Each await pauses the method until that task finishes, so the next task only starts after the previous one is done - the work becomes a queue. Starting every task first and awaiting afterwards lets them run at the same time. Bonus point - the total time drops from the SUM of all tasks to roughly the LONGEST single task.
  - q: When would you use Task.WhenAny instead?
    a: WhenAny completes when the first task finishes, so use it when you only need one outcome and the rest do not matter - like asking three servers the same question and taking the fastest answer. Bonus point - it is also the classic trick for adding a timeout by racing a task against Task.Delay.
---

## What is it?

`Task.WhenAll` takes several tasks and hands you back ONE task that finishes
when all of them have finished. It is how you let many slow jobs run at the
same time, then wait for the whole set exactly once.

## The real-world picture

Breakfast for the family. The slow way: boil the eggs and watch them. Fry the
bacon and watch it. Toast the bread and watch it. Three 5-minute jobs eat 15
minutes of your morning.

The fast way: eggs on, bacon in the pan, bread in the toaster ÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬Â all started
within a minute. Then you sit down with one combined "buzzer" that rings when
the LAST item is ready. That combined buzzer is `Task.WhenAll`.

## How it works in C#

```csharp
// SLOW - one by one (~1.5 s): each await stalls before the next job even starts.
await BoilEggsAsync();
await FryBaconAsync();
await ToastBreadAsync();

// FAST - all at once (~0.5 s): start everything first, then wait for all.
Task eggs  = BoilEggsAsync();   // starts RIGHT AWAY, hands back a receipt
Task bacon = FryBaconAsync();   // starts too
Task toast = ToastBreadAsync(); // starts too
await Task.WhenAll(eggs, bacon, toast); // one buzzer for the whole set
```

The key idea: **calling** an async method already starts the work ÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬Â the receipt
is "hot". `await` never starts anything; it only waits. So start all three,
then await the combined receipt.

`Task.WhenAll` has a cousin worth knowing: `Task.WhenAny` waits for the FIRST
task to finish ÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬Â handy when several workers race and you only need the fastest.

## See it move

Press **Run demo**. Round 1 awaits one-by-one: three work spans form a neat
staircase, about 1.8 s. Round 2 uses `Task.WhenAll`: the same three spans STACK
on top of each other, about 0.6 s. Same cooking, a third of the time.

## Watch out

- You might `await` each task before starting the next one. The tasks never
  overlap ÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬Â you get the staircase, not the stack. Start everything first.
- You might think `WhenAll` starts the tasks. They are already running ÃƒÂ¢Ã¢â€šÂ¬Ã¢â‚¬Â
  `WhenAll` only waits for the set.
- You might use `WhenAll` when you actually want the FIRST finisher. That is
  `Task.WhenAny`.

## Key takeaways

- `Task.WhenAll(...)` = one task that completes when ALL given tasks complete.
- Calling an async method starts it; awaiting only waits. Start all, then await all.
- Parallel waiting turns 1.5 s of chores into 0.5 s.
- `Task.WhenAny(...)` waits for the FIRST finisher.
- Remember the order: start first, await second.