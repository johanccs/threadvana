using System.Threading;

public static class Demo
{
    public static void RunAsync()
    {
        var loader = new Thread(LoadData) { Name = "data-loader" };
        var warmer = new Thread(WarmCache) { Name = "cache-warmer" };
        var config = new Thread(LoadConfig) { Name = "config-loader" };

        loader.Start();
        warmer.Start();
        config.Start();

        loader.Join();
        warmer.Join();
        config.Join();
    }

    static void LoadData()
    {
        Trace.Log("work-start", "loading data file");
        Thread.Sleep(200);
        Trace.Log("work-end", "data loaded");
    }

    static void WarmCache()
    {
        Trace.Log("work-start", "warming cache");
        Thread.Sleep(150);
        Trace.Log("work-end", "cache warm");
    }

    static void LoadConfig()
    {
        Trace.Log("work-start", "phase 1 - reading config");
        Thread.Sleep(100);
        Trace.Log("work-end", "phase 2 - connecting");
        Thread.Sleep(200);
        Trace.Log("work-start", "phase 3 - done");
        Thread.Sleep(100);
        Trace.Log("work-end", "config complete");
    }
}