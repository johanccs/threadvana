using System.Threading.Tasks;

public static class Solution
{
    public static async Task WorkerAsync(int id)
    {
        await Task.Delay(100 * id);
        System.Console.WriteLine($"Worker {id} done.");
    }

    public static async Task LaunchWorkers()
    {
        var w1 = WorkerAsync(1);
        var w2 = WorkerAsync(2);
        await Task.WhenAll(w1, w2);
    }
}
