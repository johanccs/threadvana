using System.Collections.Concurrent;

public static class Solution
{
    public static readonly ConcurrentDictionary<string, int> Scores = new();

    /// <summary>Atomically add points to a player's score.</summary>
    public static void RecordScore(string player, int points)
    {
        // TODO: use Scores.AddOrUpdate(key, addFactory, updateFactory)
        Scores[player] = Scores.GetValueOrDefault(player) + points;
    }

    /// <summary>Get a player's score (0 if unknown).</summary>
    public static int GetScore(string player)
    {
        // TODO: use Scores.TryGetValue(key, out int score)
        return 0;
    }
}
