using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using thefirey33_backend.Types.Database.Request;

namespace thefirey33_backend.Types.Database.Response;

public class ArtDbResponse : ArtDbRequest
{
    /// <summary>
    ///     The ID.
    /// </summary>
    [JsonPropertyName("id")]
    public int Id { get; init; }

    /// <summary>
    ///     The UUID of this object.
    /// </summary>
    [JsonPropertyName("uuid")]
    [MaxLength(256)]
    public required string Uuid { get; init; }
}