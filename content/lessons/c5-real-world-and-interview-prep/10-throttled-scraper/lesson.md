---
id: c5-l10-throttled-scraper
category: c5-real-world-and-interview-prep
order: 10
title: "Build It: A Throttled Parallel Web Scraper"
difficulty: advanced
description: "Build a throttled web scraper: control concurrent HTTP requests with a SemaphoreSlim to avoid overwhelming servers."
explainer: semaphore
interview:
  - q: "Design a web scraper that respects a max-concurrency limit and per-domain throttling."
    a: "Use a SemaphoreSlim for global concurrency, a ConcurrentDictionary of per-domain SemaphoreSlims for domain-level throttling, and HttpClient with CancellationToken for timeouts. Queue URLs in a Channel, and have N workers drain the channel, each awaiting the domain semaphore before hitting the URL. This keeps total parallelism capped and prevents hammering any single domain."
---

## What is it?

A real-world producer-consumer with backpressure: a `Channel<string>` of URLs, N worker tasks, a global `SemaphoreSlim(N)` to cap total in-flight requests, and per-domain semaphores for politeness. This is the pattern every production scraper, crawler, or batch API caller uses.

## Key takeaways

- Channel for URL queue; SemaphoreSlim for concurrency cap.
- Per-domain semaphores prevent hammering a single host.
- CancellationToken for timeout + graceful shutdown.
