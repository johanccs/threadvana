Build a `Solution.RateLimiter` class that allows at most N actions per second.

The checker creates your limiter with `new RateLimiter(N)`, calls `TryActionAsync()` from many
parallel tasks, and verifies that in any one-second window, at most N calls returned `true`.

Your `RateLimiter` must have:
- A constructor `RateLimiter(int maxPerSecond)` â   stores the limit, starts a refill timer.
- A method `Task<bool> TryActionAsync()` â   returns `true` when an action is allowed,
  `false` when throttled. Must be thread-safe and never block indefinitely.

Use whatever primitives you need â   `SemaphoreSlim` is the star, but a `Timer` and a counter
will join the cast.

## Hints

1. The `maxPerSecond` tells you the refill interval: `1000ms / maxPerSecond`.
2. A `Timer` adds tokens; `TryActionAsync` checks-and-decrements under a small lock (a `SemaphoreSlim(1)` works).
3. No external dependencies â   just `System.Threading` and `System.Threading.Tasks`.
