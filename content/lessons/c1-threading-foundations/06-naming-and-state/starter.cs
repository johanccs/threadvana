using System;
using System.Threading;

public static class Solution
{
    // The checker reads these after Run() returns.
    public static bool Ran = false;
    public static string WorkerName = null;

    // Provided: runs on YOUR thread and reports who it ran on.
    public static void Work()
    {
        Thread.Sleep(200); // pretend to work
        WorkerName = Thread.CurrentThread.Name; // null if the thread has no name!
        Ran = true;
    }

    public static void Run()
    {
        // TODO: 1. Create a new Thread that runs Work.
        //       2. Give it the name "data-worker", right where you create it
        //          (a thread's name can only be set once).
        //       3. Start() it.
        //       4. Join() it, so Run() returns only when Work is done.
    }
}
