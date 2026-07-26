using System.Collections.Concurrent;

public static class Solution
{
    public static readonly ConcurrentDictionary<string, int> Scores = new();

    public static void RecordHit(string page)
    {
        // TODO: AddOrUpdate
    }

    public static int GetHits(string page)
    {
        // TODO: TryGetValue
        return 0;
    }
}
