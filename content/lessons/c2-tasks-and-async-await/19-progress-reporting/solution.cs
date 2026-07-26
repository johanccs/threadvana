using System;
using System.Threading.Tasks;

public static class Solution
{
    public static async Task<string> RunWithProgressAsync(IProgress<int> progress)
    {
        for (var i = 0; i <= 5; i++)
        {
            progress.Report(i * 20);
            await Task.Delay(100);
        }
        return "done";
    }
}
