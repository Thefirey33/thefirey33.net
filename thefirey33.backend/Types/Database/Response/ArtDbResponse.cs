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

    /// <summary>
    /// Get ArtDbResponse from ArtDbType Object.
    /// </summary>
    /// <param name="artDbType">The ArtDbType from the Database.</param>
    /// <returns>Response Object.</returns>
    public static ArtDbResponse GetFrom(ArtDbType artDbType)
    {
        return new ArtDbResponse
        {
            Description = artDbType.Description,
            Id = artDbType.Id,
            Author = artDbType.Author,
            Category = artDbType.Category,
            Title = artDbType.Title,
            Uuid = artDbType.Uuid
        };
    }
}