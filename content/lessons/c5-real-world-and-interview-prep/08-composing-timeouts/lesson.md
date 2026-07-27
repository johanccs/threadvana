---
id: c5-l08-composing-timeouts
category: c5-real-world-and-interview-prep
order: 8
title: "Composing Timeouts and Cancellation in Real API Calls"
difficulty: advanced
description: "Compose timeouts with CancellationTokenSource.CancelAfter: every async operation gets a deadline, no exceptions."
visualization: async-activity
interview:
  - q: "How do you combine a user-cancellation, a timeout, and an async API call into one operation?"
    a: "Create a linked CancellationTokenSource from the user's token and a timeout source. Pass the linked token to the API call. Use Task.WhenAny to race the call against Task.Delay(timeout). If the delay wins, cancel the linked source (which cancels both). If the call wins, dispose the timeout source. This gives you user-triggered cancellation AND an automatic safety timeout."
  - q: "Why use a linked CancellationTokenSource instead of two separate tokens?"
    a: "Linked tokens combine into one  -  if EITHER source is cancelled (user hits cancel, OR timeout fires), the combined token signals. The API only needs to check one token. This keeps the code simple and avoids race conditions between two separate token checks."
---

## What is it?

Real production calls layer three things: the user's cancellation (they closed the tab), a safety timeout (the call must not hang forever), and the actual async call. Linked `CancellationTokenSource` ties them together so any one can cancel the operation.

## Key takeaways

- `CancellationTokenSource.CreateLinkedTokenSource(userToken, timeoutToken)` → either cancels.
- `Task.WhenAny(call, Task.Delay(timeout))` → detect timeout, cancel linked source.
- Dispose both sources after the call.
