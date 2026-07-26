using System;
using System.Threading.Tasks;

public static class __Harness
{
    private static int _attempts;

    public static async Task<HarnessResult> ValidateAsync()
    {
        var result = new HarnessResult();
        _attempts = 0;
        var answer = await Solution.RetryWithBackoffAsync(() =>
        {
            _attempts++;
            if (_attempts < 3) throw new InvalidOperationException();
            return Task.FromResult("ok");
        }, 3);
        result.Add("retry-succeeds", answer == "ok", "ok", answer,
            answer != "ok" ? "Retry 3 times with backoff — the 3rd attempt should succeed." : "");
        return result;
    }
}
