using System.Collections.Concurrent;

public static class Solution
{
    public static readonly ConcurrentDictionary<string, int> Scores = new();

    public static void RecordHit(string page)
        => Scores.AddOrUpdate(page, _ => 1, (_, old) => old + 1);

    public static int GetHits(string page)
        => Scores.TryGetValue(page, out var c) ? c : 0;
}
