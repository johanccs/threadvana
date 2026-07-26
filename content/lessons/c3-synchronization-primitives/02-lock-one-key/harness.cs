using System;
using System.Threading;
using System.Threading.Tasks;

public static class __Harness
{
    public static async Task<HarnessResult> ValidateAsync()
    {
        const int rounds = 3;
        const int threads = 4;
        const int transfersPerThread = 250;
        const int amount = 10;

        var totals = new int[rounds];
        int lowestA = int.MaxValue;
        int lastB = 1000;
        bool moneyMoved = false;

        // Hammer the transfer: 4 threads x 250 transfers, several rounds.
        // A correct lock keeps the sum constant EVERY round - races are flaky,
        // so one lucky round is not enough.
        for (int r = 0; r < rounds; r++)
        {
            Solution.Reset(); // fresh 1000 + 1000 for every round

            var workers = new Thread[threads];
            for (int i = 0; i < threads; i++)
            {
                workers[i] = new Thread(() =>
                {
                    for (int t = 0; t < transfersPerThread; t++)
                        Solution.Transfer(amount);
                });
                workers[i].Name = "hammer-" + i;
            }
            foreach (var w in workers) w.Start();
            foreach (var w in workers) w.Join();

            totals[r] = Solution.AccountA + Solution.AccountB;
            if (Solution.AccountA < lowestA) lowestA = Solution.AccountA;
            lastB = Solution.AccountB;
            if (Solution.AccountB != 1000) moneyMoved = true; // B started at 1000 each round
        }

        var result = new HarnessResult();

        result.Add(
            name: "money-moved",
            passed: moneyMoved,
            expected: "transfers actually happened (AccountB changed from its 1000 start)",
            actual: $"AccountB after the last round = {lastB}",
            message: "No money ever moved. Did you delete the transfer body? Keep the if-check, subtract and add - " +
                     "just wrap them in  lock (Solution.Gate) { ... }.");

        bool totalConstant = true;
        foreach (var t in totals) if (t != 2000) totalConstant = false;

        result.Add(
            name: "total-stays-constant",
            passed: totalConstant,
            expected: "AccountA + AccountB == 2000 after EVERY round (money only moves, never appears or vanishes)",
            actual: $"round totals: {string.Join(", ", totals)}",
            message: "Money was created or destroyed! Most likely cause: the subtract and the add interleaved - " +
                     "one thread's subtract was lost while its add still landed. Wrap the WHOLE if-block " +
                     "(check, subtract, add) in  lock (Solution.Gate) { ... }.");

        result.Add(
            name: "never-overdrawn",
            passed: lowestA >= 0,
            expected: "AccountA never went negative, even under 4-thread load",
            actual: $"lowest AccountA seen: {lowestA}",
            message: "The balance went NEGATIVE. Two threads both passed the  if (AccountA >= amount)  check before either " +
                     "subtracted - the check and the moves must be inside the SAME lock, not just the moves.");

        await Task.CompletedTask; // harness shape: ValidateAsync is async
        return result;
    }
}