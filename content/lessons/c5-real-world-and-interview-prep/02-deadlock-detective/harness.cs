using System;
using System.Threading.Tasks;

public static class __Harness
{
    public static async Task<HarnessResult> ValidateAsync()
    {
        await Task.Delay(20);
        Solution.Run();
        await Task.Delay(100);

        var result = new HarnessResult();

        result.Add(
            name: "no-deadlock",
            passed: !Solution.Deadlocked,
            expected: "both threads finished (no deadlock)",
            actual: Solution.Deadlocked ? "DEADLOCKED" : "both finished",
            message: "The threads deadlocked! Thread 2 locks B then A — swap the order " +
                     "so it locks A then B like Thread 1. Consistent lock ordering prevents deadlocks.");

        return result;
    }
}
