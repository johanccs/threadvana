---
id: c3-l07-lock-best-practices
category: c3-synchronization-primitives
order: 7
title: "lock Best Practices  -  What to Lock On"
difficulty: beginner
description: "Learn lock best practices: what to lock on, how to keep lock sections short, and the mistakes that cause deadlocks."
explainer: lock-key
interview:
  - q: "What should you use as a lock object?"
    a: "A private readonly object specifically created for locking  -  `private readonly object _lock = new();` Never lock on this, typeof(...), a string (it may be interned and shared across AppDomains), or a value type (it gets boxed to a different object each time, so every lock silently does nothing). The lock object must be a reference type, private (no external code can lock it and deadlock you), and readonly (you never accidentally reassign it)."
  - q: "Why is lock(this) discouraged?"
    a: "Because external code can also lock on the same instance, creating deadlocks you cannot predict or prevent. If your class is public and someone else locks on an instance of it, your internal lock(this) blocks forever. Use a private dedicated lock object instead."
---

## What is it?

`lock` is the simplest synchronisation primitive in C# Ã¢â‚¬â€ one thread enters, others wait. But three details decide whether your lock is safe or a time bomb: what you lock ON, what you do INSIDE the lock, and how long you hold it.

## The lock object rules

| Rule | Good | Bad |
|------|------|-----|
| **Private** | `private readonly object _gate = new();` | `lock(this)`, `lock(typeof(Foo))` |
| **Reference type** | Any `object` | Value types (box silently Ã¢â€ â€™ no lock) |
| **Readonly** | `readonly` field | Reassignable Ã¢â‚¬â€ lock silently switches object |
| **Dedicated** | One object per lock domain | Strings, `this`, `typeof()` |

## The critical section rules

```csharp
private readonly object _gate = new();

public void Transfer(Account from, Account to, decimal amount)
{
    // SAFE: lock is brief, no blocking calls inside.
    lock (_gate)
    {
        from.Balance -= amount;
        to.Balance += amount;
    }

    // DANGEROUS: await inside lock Ã¢â‚¬â€ the thread may change after await.
    // Use SemaphoreSlim(1,1).WaitAsync() instead.
}
```

## Watch out

> **Never `await` inside a `lock`.** The thread that enters the lock may not be the same thread that returns from `await` Ã¢â‚¬â€ the lock will not be released on the original thread, causing a violation.

> **Keep locks SHORT.** A lock held for 100ms blocks every other thread that touches that object. Never do I/O or heavy computation inside a lock.

> **Don't lock on interned strings.** `"MyLock"` is shared across the process Ã¢â‚¬â€ multiple classes locking on the same string will accidentally serialise with each other.

## Key takeaways

- Lock object: `private readonly object _gate = new();`
- Never `this`, `typeof(...)`, strings, or value types.
- Keep the critical section brief; no I/O, no `await`, no heavy work.
- If you need async inside a critical section, use `SemaphoreSlim(1,1)`.
