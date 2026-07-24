using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using thefirey33_backend.Types.Database.Response;

namespace thefirey33_backend.Types.Database;

public class ArtDbType : ArtDbResponse
{
    /// <summary>
    ///     The reference to the file in the local filesystem.
    /// </summary>
    [JsonPropertyName("filepath")]
    [MaxLength(256)]
    public required string FilePath { get; init; }
}