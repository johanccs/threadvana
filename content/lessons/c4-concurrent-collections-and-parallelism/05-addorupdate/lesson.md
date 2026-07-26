---
id: c4-l05-addorupdate
category: c4-concurrent-collections-and-parallelism
order: 5
title: "AddOrUpdate Ã Â¢Ã¢â  Â¬ the Upsert Done Right"
difficulty: intermediate
description: "Master AddOrUpdate and GetOrAdd: the atomic get-or-create patterns that prevent duplicate work in concurrent scenarios."
explainer: race-interleaving
interview:
  - q: "What is the difference between GetOrAdd and AddOrUpdate?"
    a: "GetOrAdd is 'give me the value for this key, creating it if missing' Ã Â¢Ã¢â  Â¬ the factory may run redundantly. AddOrUpdate is 'insert if new, update if exists' Ã Â¢Ã¢â  Â¬ and the update factory runs ONCE per key, under internal locking, so it's safe for side-effect-bearing operations. Both are atomic per key but not across keys."
  - q: "When should you use AddOrUpdate instead of GetOrAdd + TryUpdate?"
    a: "AddOrUpdate is a single atomic call that handles both insert and update Ã Â¢Ã¢â  Â¬ it's shorter and avoids the check-then-act race. If the update logic depends on the old value, AddOrUpdate's updateFactory receives (key, oldValue), making it correct without an external lock."
---

## What is it?

`AddOrUpdate` is the thread-safe "upsert" for `ConcurrentDictionary`. You give it two factories: one to CREATE the value when the key is missing, and one to UPDATE it when the key exists. The update receives the old value, so you can compute `old + delta` atomically.

Unlike `GetOrAdd`, the update factory never runs redundantly Ã Â¢Ã¢â  Â¬ it's guarded by per-key locking.

## Watch out

> **The updateFactory returns the NEW value, it doesn't mutate in-place.** If the old value is a mutable object, `AddOrUpdate` does not prevent another thread from reading a stale reference Ã Â¢Ã¢â  Â¬ use immutable structures alongside it.

## Key takeaways

- `AddOrUpdate(key, addFactory, updateFactory)` Ã Â¢Ã¢â  Â¬ atomic upsert.
- Update factory gets `(key, oldValue)` Ã Â¢Ã¢â ¬Â  compute and return new.
- Safer for side-effect-heavy updates than `GetOrAdd`.
