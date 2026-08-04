using System.Text.Json.Serialization;

namespace thefirey33_backend.Types.Database.Dex;

public class LeaderboardResponse
{
    /// <summary>
    ///     The author in the leaderboard.
    /// </summary>
    [JsonPropertyName("author")]
    public required string Author { get; init; }

    /// <summary>
    ///     The amount of times they made a nikosona.
    /// </summary>
    [JsonPropertyName("count")]
    public int Count { get; init; }
}