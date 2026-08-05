using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace thefirey33_backend.Types.Database;

public class ScoreDbType
{
    /// <summary>
    ///     The ID.
    /// </summary>
    [JsonPropertyName("id")]
    public int Id { get; set; }

    /// <summary>
    ///     The name of the person who has that highscore.
    /// </summary>
    [JsonPropertyName("name")]
    public required string Name { get; set; }

    /// <summary>
    ///     The value of the highscore.
    /// </summary>
    [JsonPropertyName("value")]
    public int Value { get; set; }
}