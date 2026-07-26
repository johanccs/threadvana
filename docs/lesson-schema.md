# Lesson Schema (the content contract — read FULLY before writing any lesson)

## Folder layout

```
content/lessons/
  categories.json                     ← the 5 categories (id, order, title, description)
  <categoryId>/
    <order>-<slug>/                   ← e.g. 01-what-is-a-thread (folder name is cosmetic)
      lesson.md                       ← REQUIRED: YAML front matter + theory Markdown
      demo.cs                         ← optional: runnable visual demo
      exercise.md                     ┐ exercise set: provide ALL FOUR or NONE
      starter.cs                      │
      harness.cs                      │
      solution.cs                     ┘
```

Rules enforced by the loader (it throws at startup if violated):
- `lesson.md` must start with a `---` front matter block closed by `---`.
- Lesson `id` is unique across the whole course; format `c<cat>-l<nn>-<slug>`,
  e.g. `c1-l01-what-is-a-thread`.
- `order` is unique within a category, 1-based, no gaps.
- Front matter `category` must equal the parent folder name.

## lesson.md front matter

```yaml
---
id: c1-l01-what-is-a-thread
category: c1-threading-foundations
order: 1
title: What is a Thread?
difficulty: beginner            # beginner | intermediate | advanced
visualization: thread-timeline  # optional: thread-timeline | thread-pool | semaphore | async-activity
explainer: thread-basics        # optional: animated concept walkthrough shown above the live
                                # trace. Built-in ids: thread-basics | thread-join | thread-pool |
                                # semaphore | async-state-machine. If omitted, thread-pool/semaphore/
                                # async-activity visualizations get their matching explainer by default;
                                # thread-timeline gets none (too generic) — set it explicitly.
interview:                      # optional, 1-3 entries
  - q: What is a thread?
    a: A thread is a worker that runs one piece of code at a time. Every C# program starts on one main thread; you can start more to do work at the same time.
---
```

Theory body follows docs/writing-style.md (fixed skeleton: What is it? → The
real-world picture → How it works in C# → See it move → Watch out → Key takeaways).

## demo.cs contract (runnable visual demo)

```csharp
public static class Demo
{
    public static async Task RunAsync() { ... }
}
```

- Top `using`s allowed: System, System.Collections.Generic, System.Linq,
  System.Threading, System.Threading.Tasks (implicit usings are NOT on for
  submitted code — always write the usings you need).
- Use `Trace.Log(kind, label)` to feed the visualization. Kinds (string literals,
  from TraceKinds): `thread-start`, `thread-end`, `work-start`, `work-end`,
  `wait-start`, `wait-end`, `lock-acquire`, `lock-release`, `semaphore-enter`,
  `semaphore-exit`, `pool-queued`, `pool-dequeued`, `message`.
- Demos must finish in < 5 seconds, must not block forever, and every
  started thread must be Joined/awaited before RunAsync returns.

## Exercise contracts

**starter.cs / solution.cs** — the learner's code shape is ALWAYS:

```csharp
public static class Solution
{
    // fields/methods defined by the lesson; harness calls them.
}
```

**harness.cs** — hidden validation code, ALWAYS:

```csharp
public static class __Harness
{
    public static async Task<HarnessResult> ValidateAsync()
    {
        var result = new HarnessResult();
        result.Add(
            name: "check-id",                      // short, kebab-case
            passed: /* bool */,
            expected: "what correct code does",     // plain English
            actual: $"what the learner's code did ({...})",
            message: "what went wrong + most likely cause + what to try"); // junior-friendly!
        return result;
    }
}
```

`HarnessResult` / `HarnessCheck` / `Trace` come from the injected **prelude**
(see docs/architecture.md §Prelude) — do NOT define them in harness.cs.

Harness rules:
- Check **behavior**, not output text. Examples: observed max concurrency ≤ N;
  a flag was set by a *different* thread id; counter equals expected total after
  N parallel increments (catches races); a lock section was never entered by two
  threads at once.
- Give the learner's code a fair chance: small `Task.Delay` grace where timing is
  inherently racy, or loop-until-condition with a deadline.
- Default timeout is 10 s; a hanging submission fails as a friendly timeout
  ("this usually means a deadlock or a thread that never finishes").
- 2–5 checks per exercise, ordered from basic to subtle. First check should be
  "did anything happen at all" so a blank TODO fails gently.
- solution.cs MUST pass its own harness (the test suite runs every reference
  solution through the real pipeline and asserts Passed).

## exercise.md

Short. Task statement (numbered steps if >1), the exact `Solution` member
signatures to fill in, and 1–3 `Hints` listed at the bottom like:

```markdown
## Hints
1. First hint (nudge, not answer).
2. Second hint (more concrete).
```

(Hints are parsed from the `## Hints` section, one per numbered line.)
