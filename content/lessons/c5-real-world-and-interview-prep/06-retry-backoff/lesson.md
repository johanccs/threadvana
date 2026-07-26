---
id: c5-l06-retry-backoff
category: c5-real-world-and-interview-prep
order: 6
title: "Async Retry with Exponential Backoff ÃƒÆ’Ã‚Â¢ÃƒÂ¢Ã¢â‚¬Å¡Ã‚Â¬ÃƒÂ¢Ã¢â€šÂ¬Ã‚Â Surviving Transient Failures"
difficulty: intermediate
description: "Implement retry with exponential backoff: handle transient failures gracefully without hammering the failing resource."
visualization: async-activity
interview:
  - q: "How do you implement retry with exponential backoff?"
    a: "Loop N times: try the operation; if it succeeds, break. If it throws a transient exception (HttpRequestException, TimeoutException, SqlException with retryable codes), await Task.Delay(initialDelay ÃƒÆ’Ã†'ÃƒÂ¢Ã¢â€šÂ¬" (2^attempt)), then retry. The backoff doubles each attempt: 100ms ÃƒÆ’Ã‚Â¢ÃƒÂ¢Ã¢â€šÂ¬Ã‚Â ÃƒÂ¢Ã¢â€šÂ¬Ã¢â€žÂ¢ 200ms ÃƒÆ’Ã‚Â¢ÃƒÂ¢Ã¢â€šÂ¬Ã‚Â ÃƒÂ¢Ã¢â€šÂ¬Ã¢â€žÂ¢ 400ms ÃƒÆ’Ã‚Â¢ÃƒÂ¢Ã¢â€šÂ¬Ã‚Â ÃƒÂ¢Ã¢â€šÂ¬Ã¢â€žÂ¢ 800ms. Add jitter (random ÃƒÆ’Ã¢â‚¬Å¡Ãƒâ€šÃ‚Â±25%) to avoid synchronised retry storms across instances."
  - q: "What exceptions should you NOT retry?"
    a: "Validation errors (400 Bad Request), authentication failures (401/403), resource-not-found (404), and business-logic exceptions. Retrying these wastes resources and may make the problem worse. Retry only transient failures ÃƒÆ’Ã‚Â¢ÃƒÂ¢Ã¢â‚¬Å¡Ã‚Â¬ÃƒÂ¢Ã¢â€šÂ¬Ã‚Â network timeouts, throttling (429), temporary server errors (503)."
---

## What is it?

Transient failures are temporary hiccups ÃƒÆ’Ã‚Â¢ÃƒÂ¢Ã¢â‚¬Å¡Ã‚Â¬ÃƒÂ¢Ã¢â€šÂ¬Ã‚Â a network glitch, a database deadlock victim, a rate-limit response. Retry with exponential backoff is the standard pattern: try, wait a bit, try again with a longer wait, give up after N attempts.

## Key takeaways

- Retry loop: N attempts, `Task.Delay` with growing duration.
- Exponential: delay ÃƒÆ’Ã†'ÃƒÂ¢Ã¢â€šÂ¬" 2^attempt. Add jitter.
- Only retry transient errors; fail fast on permanent ones.
