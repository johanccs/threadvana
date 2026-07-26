You are implementing a simple library method `Solution.FetchFromCacheOrSourceAsync()`.

A helper `Solution.QueryCacheAsync()` is provided (returns a cached string or null).
A helper `Solution.QuerySourceAsync()` fetches from the real source.

Your job: call `QueryCacheAsync` first with `ConfigureAwait(false)`. If the result is
not null, return it. Otherwise, call `QuerySourceAsync` (also with ConfigureAwait(false))
and return its result.

Both helpers are library-style async methods â   they should be awaited with
ConfigureAwait(false) inside YOUR method.

## Hints
1. `var cached = await Solution.QueryCacheAsync().ConfigureAwait(false);` â   chained on the Task.
2. If `cached` is not null, return it. Otherwise await `QuerySourceAsync().ConfigureAwait(false)`.
3. The method is a library utility â   no UI, no ASP.NET â   so ConfigureAwait(false) is exactly right.
