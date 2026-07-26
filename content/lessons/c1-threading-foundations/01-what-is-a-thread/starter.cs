using System;
using System.Threading;

public static class Solution
{
    // The checker reads this after Run() returns.
    // Your new thread must store its own thread id here.
    public static int WorkerThreadId = 0;

    public static void Run()
    {
        // TODO: 1. Create a new Thread. Inside its work, set
        //          WorkerThreadId = Environment.CurrentManagedThreadId;
        //       2. Start the thread.
        //       3. Join it, so Run() waits until the work is done.
    }
}
