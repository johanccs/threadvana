using System;
using System.Threading;
using System.Threading.Tasks;

public static class __Harness
{
    public static async Task<HarnessResult> ValidateAsync()
    {
        Solution.Run();

        // Fair chance: if threads were started but not Joined, give them a
        // moment to finish writing before we look at the results.
        await Task.Delay(500);

        var result = new HarnessResult();

        // How many of each value landed in the three work slots?
        int zeros = Count(0), ones = Count(1), twos = Count(2);

        result.Add(
            name: "threads-stored-something",
            passed: Solution.Results[0] != -1 || Solution.Results[1] != -1 ||
                    Solution.Results[2] != -1 || Solution.Results[3] != -1,
            expected: "your three threads store numbers into Solution.Results",
            actual: $"Results = [{Solution.Results[0]}, {Solution.Results[1]}, " +
                    $"{Solution.Results[2]}, {Solution.Results[3]}]",
            message: "Nothing was stored anywhere. Did you create the three threads AND call Start() " +
                     "on each one? A thread that is never started never runs.");

        result.Add(
            name: "each-index-stored-once",
            passed: zeros == 1 && ones == 1 && twos == 1,
            expected: "Results slots 0-2 contain 0, 1 and 2, each exactly once",
            actual: $"slots 0-2 = [{Solution.Results[0]}, {Solution.Results[1]}, {Solution.Results[2]}] " +
                    $"(0 appears {zeros}x, 1 appears {ones}x, 2 appears {twos}x)",
            message: "Each thread must store its OWN number. If numbers are missing or doubled, the " +
                     "threads all read the same shared loop variable. Fix: inside the loop write " +
                     "int mine = i; and use mine everywhere in the thread body.");

        result.Add(
            name: "tripwire-untouched",
            passed: Solution.Results[3] == -1,
            expected: "Results[3] stays -1 (it is a tripwire - no correct thread ever writes it)",
            actual: $"Results[3] = {Solution.Results[3]}",
            message: "The tripwire was written - a thread used the loop variable AFTER the loop had " +
                     "already finished (i was 3 by then). Classic capture bug! Fix: int mine = i; " +
                     "inside the loop, then use mine in the thread body.");

        return result;
    }

    // Counts how often a value appears in the three work slots (0-2).
    private static int Count(int value)
    {
        int n = 0;
        for (int i = 0; i < 3; i++)
            if (Solution.Results[i] == value) n++;
        return n;
    }
}
