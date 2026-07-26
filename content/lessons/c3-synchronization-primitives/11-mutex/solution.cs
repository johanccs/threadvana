using System.Threading;
using System.Threading.Tasks;

public static class Solution
{
    public static Task<string> TryAcquireSingleInstanceAsync()
    {
        using var mutex = new Mutex(false, @"Global\ThreadCraftExercise", out var createdNew);
        return Task.FromResult(createdNew ? "first" : "second");
    }
}
