---
id: c5-l09-backgroundservice
category: c5-real-world-and-interview-prep
order: 9
title: "BackgroundService  -  Long-Running Workers in ASP.NET Core"
difficulty: advanced
description: "Use IHostedService and BackgroundService: the ASP.NET way to run long-running background work safely."
explainer: channel
interview:
  - q: "What is BackgroundService in ASP.NET Core?"
    a: "It is a base class for long-running background tasks. You override ExecuteAsync(CancellationToken)  -  the framework starts it when the app starts and stops it (via the cancellation token) on graceful shutdown. Use it for queue processing, timed cleanup, or any loop that should run for the lifetime of the application. Register it with AddHostedService<T>()."
  - q: "What is the difference between IHostedService and BackgroundService?"
    a: "IHostedService is the raw interface (StartAsync/StopAsync). BackgroundService simplifies it  -  you only override ExecuteAsync and the base class handles Start/Stop lifecycle. For simple looping tasks, use BackgroundService."
---

## What is it?

A `BackgroundService` is how you run a perpetual loop in ASP.NET Core: a queue processor that drains items forever, a periodic cleanup that runs every hour, or a health-check pinger. The framework starts it at boot, cancels it on shutdown, and handles exceptions so a single crash doesn't kill the process.

## Key takeaways

- `public class MyWorker : BackgroundService` Ã¢â‚¬â€ override `ExecuteAsync`.
- `while (!stoppingToken.IsCancellationRequested) { ... }` Ã¢â‚¬â€ the standard loop.
- Register: `builder.Services.AddHostedService<MyWorker>();`
