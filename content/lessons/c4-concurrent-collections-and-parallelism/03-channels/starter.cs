using System;
using System.Threading.Channels;
using System.Threading.Tasks;

public static class Solution
{
    public static string LastMessage = "";

    public static async Task RunAsync()
    {
        // TODO: create a bounded Channel<string>(1)
        // TODO: producer task: write "hello", "world", "done", then Complete()
        // TODO: consumer task: read all messages, store last in LastMessage
        // TODO: await both tasks

        await Task.CompletedTask;
    }
}
