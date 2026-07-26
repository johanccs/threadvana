using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public static class Solution
{
    public static Task<string> RunCancellableLoopAsync(CancellationToken token)
    {
        try
        {
            var opts = new ParallelOptions { CancellationToken = token };
            Parallel.For(0, 100, opts, i =>
            {
                token.ThrowIfCancellationRequested();
                Thread.Sleep(1);
            });
            return Task.FromResult("done");
        }
        catch (OperationCanceledException)
        {
            return Task.FromResult("cancelled");
        }
        catch (AggregateException ae) when (ae.InnerExceptions.Any(e => e is OperationCanceledException))
        {
            return Task.FromResult("cancelled");
        }
    }
}
