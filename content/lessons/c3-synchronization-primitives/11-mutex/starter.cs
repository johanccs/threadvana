using System.Threading;
using System.Threading.Tasks;

public static class Solution
{
    /// <summary>Return "first" if this is the first instance, "second" otherwise.</summary>
    public static Task<string> TryAcquireSingleInstanceAsync()
    {
        // TODO: new Mutex with a name, check createdNew
        return Task.FromResult("not implemented");
    }
}
