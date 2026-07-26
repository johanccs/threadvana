using System.Threading;
using System.Threading.Tasks;

public static class Solution
{
    public static readonly AutoResetEvent Evt = new(false);

    public static async Task<string> SignalAndWaitAsync()
    {
        Evt.Set();
        await Task.Delay(100);
        return "signalled";
    }
}
