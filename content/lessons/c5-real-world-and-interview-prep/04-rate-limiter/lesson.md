---
id: c5-l04-rate-limiter
category: c5-real-world-and-interview-prep
order: 4
title: Rate Limiter Ã¢â‚¬â€ the Token Bucket with SemaphoreSlim
difficulty: intermediate
description: "Build a thread-safe rate limiter: control how many operations per second your code can perform using semaphores and timers."
visualization: semaphore
interview:
  - q: Design a rate limiter that allows N requests per second. What synchronization primitive would you use?
    a: A token-bucket with SemaphoreSlim(N) Ã¢â‚¬â€ a timer releases one token every 1/N seconds. SemaphoreSlim.WaitAsync blocks incoming requests when the bucket is empty; the passing requests decrease the count. This is the answer most system-design interviews expect.
  - q: What happens when you use a simple lock + counter instead of a semaphore for rate limiting?
    a: A counter can't block Ã¢â‚¬â€ requesters must poll (spin-sleep) which wastes CPU and is never precise. SemaphoreSlim natively blocks until capacity is available, and WaitAsync gives you cooperative, non-blocking waiting when called from async code.
---

## What is it?

A rate limiter controls how many actions can happen in a time window Ã¢â‚¬â€ the famous "N requests per second" you see in every API gateway. The most elegant .NET implementation is the **token bucket**: one async timer refills tokens at a steady rate; incoming requests `await` a token. If the bucket is empty, they queue naturally.

This is not a toy Ã¢â‚¬â€ this pattern runs production rate limiters in real services.

## The real-world picture

A coffee shop has exactly 3 machines. Every 200 ms, one machine finishes its brew and a bell rings Ã¢â‚¬â€ one more order can start. Customers wait in line; when a machine is free, the next customer starts. No polling, no busy-waiting Ã¢â‚¬â€ just a semaphore.

## How it works in C#

```csharp
public sealed class TokenBucket
{
    private readonly SemaphoreSlim _semaphore = new(1, MaxTokens);
    private readonly Timer _refillTimer;
    private const int MaxTokens = 10;
    private volatile int _tokens = MaxTokens;

    public TokenBucket(double tokensPerSecond)
    {
        var interval = TimeSpan.FromMilliseconds(1000.0 / tokensPerSecond);
        _refillTimer = new Timer(_ => Refill(), null, interval, interval);
    }

    private void Refill() => Interlocked.Add(ref _tokens, 1);

    public async Task<bool> TryConsumeAsync(CancellationToken ct = default)
    {
        await _semaphore.WaitAsync(ct);   // one at a time
        try
        {
            if (Volatile.Read(ref _tokens) > 0)
            {
                Interlocked.Decrement(ref _tokens);
                return true;             // token consumed
            }
            return false;                // still empty
        }
        finally { _semaphore.Release(); }
    }
}
```

Key design: the semaphore serialises the check-and-decrement Ã¢â‚¬â€ it's the *critical section* guard, not the rate limit itself; the `_tokens` count is the actual limit.

## See it move

Press **Run demo** Ã¢â‚¬â€ 20 requesters try to consume tokens from a bucket that refills at 5/s. The timeline shows requests that pass (green) and requests that are told to wait (amber). Count how many pass in the first 2 seconds.

## Watch out

> **Timer fire-and-forget catches no exceptions.** If `Refill()` throws, the timer silently stops. Keep it bullet-proof Ã¢â‚¬â€ no allocations, no external calls.

> **The semaphore is the gate guard, not the room.** With `MaxTokens = 10` and `_semaphore = new(1, 10)`, the semaphore allows only ONE at a time through the check Ã¢â‚¬â€ but up to 10 may be waiting in line.

> **For production, consider the System.Threading.RateLimiting namespace** (NET 7+): `TokenBucketRateLimiter` is built-in. But interviewers want to see you build one from primitives Ã¢â‚¬â€ hence this exercise.

## Key takeaways

- Token bucket = SemaphoreSlim serialising access + a counter + a timer.
- The semaphore is One-At-A-Time inside the critical check; the token count is the actual limit.
- For async callers, use `WaitAsync` instead of `Wait()` Ã¢â‚¬â€ no thread blocking.
