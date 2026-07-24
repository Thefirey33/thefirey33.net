using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace thefirey33_backend.Types.Database;

public class NikoDexRecoveryDbType
{
    /// <summary>
    ///     The ID.
    /// </summary>
    [JsonPropertyName("id")]
    public int Id { get; set; }

    /// <summary>
    ///     The reference to the file in the local filesystem.
    /// </summary>
    [JsonPropertyName("filepath")]
    [MaxLength(256)]
    public required string FilePath { get; init; }
}