---
id: c5-l07-circuit-breaker
category: c5-real-world-and-interview-prep
order: 7
title: "Circuit Breaker  -  Stop Calling the Dead Service"
difficulty: advanced
description: "Build a circuit breaker: detect when a downstream system is failing and stop calling it entirely to let it recover."
explainer: async-state-machine
interview:
  - q: "What is the circuit breaker pattern?"
    a: "A state machine with three states: Closed (normal  -  calls pass through), Open (failure threshold hit  -  calls fail immediately without hitting the backend), HalfOpen (after a timeout  -  one test-call is allowed to see if the backend is back). This prevents a slow/broken downstream from consuming your resources with repeated failed calls."
  - q: "How does it differ from simple retry?"
    a: "Retry assumes the failure is transient and retries within the same operation. Circuit breaker assumes the failure may be systemic  -  repeated calls will fail for a while  -  so it stops ALL calls for a cooldown period. This gives the downstream time to recover and saves your resources."
---

## What is it?

A circuit breaker wraps every call to an external dependency. When failures exceed a threshold in a time window, the breaker "opens" Ã¢â‚¬â€ all subsequent calls fail instantly without hitting the backend. After a cooldown period, it transitions to half-open Ã¢â‚¬â€ one test call goes through. If it succeeds, the breaker closes. If it fails, it stays open.

## Key takeaways

- Three states: Closed Ã¢â€ â€™ Open (threshold reached) Ã¢â€ â€™ HalfOpen (cooldown) Ã¢â€ â€™ Closed/Open.
- Fails fast when open Ã¢â‚¬â€ no cascading waits.
- Protects both your service and the downstream from overload.
