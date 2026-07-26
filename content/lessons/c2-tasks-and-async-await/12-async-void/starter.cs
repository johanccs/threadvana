using System.Threading.Tasks;

public static class Solution
{
    /// <summary>A fast worker — honour this signature, don't change.</summary>
    public static async Task WorkerAsync(int id)
    {
        await Task.Delay(100 * id);
        System.Console.WriteLine($"Worker {id} done.");
    }

    /// <summary>Change this from async void to async Task so callers can track it.</summary>
    public static async void LaunchWorkers()
    {
        var w1 = WorkerAsync(1);
        var w2 = WorkerAsync(2);
        // TODO: return a Task that represents both workers completing
    }
}
