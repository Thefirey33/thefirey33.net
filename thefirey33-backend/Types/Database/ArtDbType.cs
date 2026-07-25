using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using thefirey33_backend.Types.Database.Response;

namespace thefirey33_backend.Types.Database;

public class ArtDbType : ArtDbResponse
{
    /// <summary>
    ///     The amount of likes that this post has.
    /// </summary>
    [JsonPropertyName("likes")]
    public List<LikesDbType> Likes { get; set; } = [];

    /// <summary>
    ///     The reference to the file in the local filesystem.
    /// </summary>
    [JsonPropertyName("filepath")]
    [MaxLength(256)]
    public required string FilePath { get; init; }
}