---
id: c2-l02-async-await-keywords
category: c2-tasks-and-async-await
order: 2
title: async and await Ã Æ Ã Â¢Ã Â¢Ã¢â ¬Å¡Ã Â¬Ã Â¢Ã¢â  Â¬Ã Â  Pause Here, Come Back Later
difficulty: beginner
description: "Learn async and await: the two keywords that make asynchronous C# code read like simple synchronous code."
visualization: async-activity
explainer: async-state-machine
interview:
  - q: What do the async and await keywords do?
    a: async marks a method as one that can pause, and await marks the exact spot where it pauses. When the method awaits a task, its thread is set free to do other work; when the task finishes, the method resumes right after the await. Bonus point - the compiler rewrites the method into a little state machine behind the scenes, which is how it knows where to resume.
  - q: What is the difference between Task and Task<T>?
    a: Task means "work that finishes later, with no result inside". Task<T> also finishes later, but it delivers a value of type T when it completes. You await both the same way - awaiting a Task<T> unwraps the T for you.
  - q: Why is async void usually a mistake?
    a: An async void method returns no receipt, so nobody can await it and nobody sees when it finishes - or when it crashes. Its exceptions escape onto the wrong context and can take the whole app down. The one accepted use is event handlers, like button clicks, which must be void.
---

## What is it?

Two keywords, one sentence each:

- **`async`** marks a method as one that *can pause* partway through.
- **`await`** marks the exact spot where it pauses: "wait for this task here Ã Æ Ã Â¢Ã Â¢Ã¢â ¬Å¡Ã Â¬Ã Â¢Ã¢â  Â¬Ã Â 
  let the thread go do other work, and resume me when it is done."

That is the whole idea. Everything else is detail.

## The real-world picture

Back to the coffee shop. You have your buzzer from *Meet the Task Ã Æ Ã Â¢Ã Â¢Ã¢â ¬Å¡Ã Â¬Ã Â¢Ã¢â  Â¬Ã Â  a Promise of
a Future Result*. Standing at the counter staring at the barista until the drink
is done would be silly Ã Æ Ã Â¢Ã Â¢Ã¢â ¬Å¡Ã Â¬Ã Â¢Ã¢â  Â¬Ã Â  you are stuck, and nobody else can be served.

`await` is sitting back down. You keep your buzzer, the counter is free for
other customers, and when the buzzer rings you pick up *exactly where you left
off* Ã Æ Ã Â¢Ã Â¢Ã¢â ¬Å¡Ã Â¬Ã Â¢Ã¢â  Â¬Ã Â  the very next step in your method.

## How it works in C#

The smallest real async method:

```csharp
using System.Threading.Tasks;

// async = this method can pause. Task<string> = it promises a string later.
public static async Task<string> MakeTeaAsync()
{
    // await = pause HERE. The thread is free while the delay runs.
    await Task.Delay(500); // pretend: waiting for the kettle

    // When the task completes, the method resumes on this very line.
    return "tea ready";    // the string rides back inside Task<string>
}
```

Return types, in one line each:

- `Task` Ã Æ Ã Â¢Ã Â¢Ã¢â ¬Å¡Ã Â¬Ã Â¢Ã¢â  Â¬Ã Â  the method finishes later, no result inside. Callers can still await it.
- `Task<T>` Ã Æ Ã Â¢Ã Â¢Ã¢â ¬Å¡Ã Â¬Ã Â¢Ã¢â  Â¬Ã Â  the method finishes later *and* delivers a `T`. `await` unwraps it.
- `async void` Ã Æ Ã Â¢Ã Â¢Ã¢â ¬Å¡Ã Â¬Ã Â¢Ã¢â  Â¬Ã Â  avoid it! Nobody gets a receipt, so nobody can await it, and a
  crash inside can kill the app. One exception: event handlers (like a button
  click) must be `void`, so `async void` is allowed there.

## See it move

Press **Run demo** and read the thread ids in the labels. The line *before* the
`await` runs on one thread. During the wait, that thread is set free Ã Æ Ã Â¢Ã Â¢Ã¢â ¬Å¡Ã Â¬Ã Â¢Ã¢â  Â¬Ã Â  look at
the grey gap. The line *after* the `await` may resume on a **different** pool
thread. Same method, new worker Ã Æ Ã Â¢Ã Â¢Ã¢â ¬Å¡Ã Â¬Ã Â¢Ã¢â  Â¬Ã Â  the state machine doesn't care.

## Watch out

- You might think `await` blocks the thread. It does the opposite Ã Æ Ã Â¢Ã Â¢Ã¢â ¬Å¡Ã Â¬Ã Â¢Ã¢â  Â¬Ã Â  it hands the
  thread back. Blocking is what `.Result` and `.Wait()` do, and that is the trap.
- You might write `async void` to make the compiler happy. Now nobody can await
  you, and your exceptions escape. Return `Task` or `Task<T>` instead.
- You might call an async method and forget `await`. The work still starts, but
  your code rushes ahead without the result Ã Æ Ã Â¢Ã Â¢Ã¢â ¬Å¡Ã Â¬Ã Â¢Ã¢â  Â¬Ã Â  like walking out before the
  buzzer rings.

## Key takeaways

- `async` = this method can pause; `await` = pause here, resume me later.
- During the pause, the thread is FREE Ã Æ Ã Â¢Ã Â¢Ã¢â ¬Å¡Ã Â¬Ã Â¢Ã¢â  Â¬Ã Â  released, not blocked.
- After the task completes, the method continues on the next line, possibly on a different thread.
- Return `Task` or `Task<T>`; keep `async void` for event handlers only.
- If you reach for `.Result` or `.Wait()`, stop Ã Æ Ã Â¢Ã Â¢Ã¢â ¬Å¡Ã Â¬Ã Â¢Ã¢â  Â¬Ã Â  you want `await`.