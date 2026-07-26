Write `Solution.CallWithLinkedCancellationAsync(CancellationToken userToken, int timeoutMs)`:

1. Create a timeout `CancellationTokenSource`.
2. Link user token + timeout token.
3. Call `Solution.SlowApiAsync(linkedToken)` inside a try/catch.
4. On `OperationCanceledException`, return `"cancelled"`. On success, return `"ok"`.

## Hints
1. `using var timeoutCts = new CancellationTokenSource(timeoutMs);`
2. `using var linked = CancellationTokenSource.CreateLinkedTokenSource(userToken, timeoutCts.Token);`
3. `await SlowApiAsync(linked.Token);`
