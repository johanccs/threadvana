using System;
using System.Collections.Generic;

public static class Solution
{
    public static int Pauses = 2;
    public static string FirstToRun = "StartToasting";
    public static string AfterFirstAwait = "AddJam";

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
