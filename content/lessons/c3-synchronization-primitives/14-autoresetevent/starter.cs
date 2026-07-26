using System.Threading;
using System.Threading.Tasks;

public static class Solution
{
    public static readonly AutoResetEvent Evt = new(false);

    public static async Task<string> SignalAndWaitAsync()
    {
        // TODO: set the event, wait 100ms, return "signalled"
        return "not implemented";
    }
}
