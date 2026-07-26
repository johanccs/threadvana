using System.Threading;
using System.Threading.Tasks;

public static class Solution
{
    public static void Shout(int waiterNumber)
    {
        Thread.Sleep(20 + waiterNumber * 5);
        System.Console.WriteLine($"Waiter {waiterNumber} placed their order.");
        Done.Signal();
    }

    public static CountdownEvent Done = new(0);

    public static async Task ShoutAllAsync()
    {
        var waiter1 = Task.Run(() => Shout(1));
        var waiter2 = Task.Run(() => Shout(2));
        var waiter3 = Task.Run(() => Shout(3));

        Done.Wait();
    }
}
