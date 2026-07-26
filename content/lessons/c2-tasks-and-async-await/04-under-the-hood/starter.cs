using System;
using System.Collections.Generic;

public static class Solution
{
    // TODO: Fill in your answers

    // How many times does MakeToastAsync() pause (return before finishing)?
    public static int Pauses = 0;

    // Which runs first: the StartToasting() call at the top, or SpreadButterAsync()?
    public static string FirstToRun = ""; // "StartToasting" or "SpreadButter"

    // After the first await finishes, what runs next?
    public static string AfterFirstAwait = ""; // "AddJam" or "return 'toast'"

    // Provided — log of what ran and in what order
    public static readonly List<string> Log = new();

    public static void Run()
    {
        Log.Clear();
    }

    public static async System.Threading.Tasks.Task<string> MakeToastAsync()
    {
        StartToasting();
        await SpreadButterAsync();
        await AddJamAsync();
        return "toast";
    }

    private static void StartToasting() => Log.Add("StartToasting");

    private static async System.Threading.Tasks.Task SpreadButterAsync()
    {
        Log.Add("SpreadButter-start");
        await System.Threading.Tasks.Task.Delay(50);
        Log.Add("SpreadButter-end");
    }

    private static async System.Threading.Tasks.Task AddJamAsync()
    {
        Log.Add("AddJam-start");
        await System.Threading.Tasks.Task.Delay(50);
        Log.Add("AddJam-end");
    }
}
