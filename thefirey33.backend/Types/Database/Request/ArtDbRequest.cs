using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace thefirey33_backend.Types.Database.Request;

public class ArtDbRequest
{
    /// <summary>
    ///     The category of this art.
    /// </summary>
    [JsonPropertyName("category")]
    public string? Category { get; init; }

    /// <summary>
    ///     The author of this art.
    /// </summary>
    [JsonPropertyName("author")]
    public required string Author { get; init; }

    /// <summary>
    ///     The Title.
    /// </summary>
    [JsonPropertyName("title")]
    [MaxLength(100)]
    public required string Title { get; init; }

    /// <summary>
    ///     The description.
    /// </summary>
    [JsonPropertyName("description")]
    [MaxLength(256)]
    public required string Description { get; init; }
}