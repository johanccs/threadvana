Write `Solution.GetValueAsync(string key)` using the single-flight pattern:

1. Use a `ConcurrentDictionary<string, Lazy<Task<string>>>` as a cache.
2. `GetOrAdd` the key â   the factory creates a new `Lazy<Task<string>>` that calls `Solution.FetchFromSourceAsync(key)`.
3. Return `await lazy.Value`.

## Hints
1. `var lazy = _cache.GetOrAdd(key, _ => new Lazy<Task<string>>(() => FetchFromSourceAsync(key)));`
2. `return await lazy.Value;`
