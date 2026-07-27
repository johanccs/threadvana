---
id: c5-l06-retry-backoff
category: c5-real-world-and-interview-prep
order: 6
title: "Async Retry with Exponential Backoff - Surviving Transient Failures"
difficulty: intermediate
description: "Implement retry with exponential backoff: handle transient failures gracefully without hammering the failing resource."
visualization: async-activity
interview:
  - q: "How do you implement retry with exponential backoff?"
    a: "Loop N times: try the operation; if it succeeds, break. If it throws a transient exception (HttpRequestException, TimeoutException, SqlException with retryable codes), await Task.Delay(initialDelay * (2^attempt)), then retry. The backoff doubles each attempt: 100ms, 200ms, 400ms, 800ms. Add jitter (random +/-25%) to avoid synchronised retry storms across instances."
  - q: "What exceptions should you NOT retry?"
    a: "Validation errors (400 Bad Request), authentication failures (401/403), resource-not-found (404), and business-logic exceptions. Retrying these wastes resources and may make the problem worse. Retry only transient failures: network timeouts, throttling (429), temporary server errors (503)."
---
## What is it?

Transient failures are temporary hiccups — a network glitch, a database deadlock victim, a rate-limit response. Retry with exponential backoff is the standard pattern: try, wait a bit, try again with a longer wait, give up after N attempts.

## Key takeaways

- Retry loop: N attempts, `Task.Delay` with growing duration.
- Exponential: delay ÃÆ'â€" 2^attempt. Add jitter.
- Only retry transient errors; fail fast on permanent ones.
