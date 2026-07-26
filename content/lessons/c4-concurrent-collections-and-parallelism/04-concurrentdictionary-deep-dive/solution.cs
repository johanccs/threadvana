using System.Collections.Concurrent;

public static class Solution
{
    public static readonly ConcurrentDictionary<string, int> Scores = new();

    public static void RecordScore(string player, int points)
        => Scores.AddOrUpdate(player, _ => points, (_, old) => old + points);

    public static int GetScore(string player)
        => Scores.TryGetValue(player, out var score) ? score : 0;
}
