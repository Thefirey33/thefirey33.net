using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace thefirey33_backend.Types.Database;

public class ApprovalDbType
{
    /// <summary>
    ///     The ID of this database entry.
    /// </summary>
    [JsonPropertyName("id")]
    public int Id { get; init; }

    /// <summary>
    ///     The UUID of the Minecraft user.
    /// </summary>
    [JsonPropertyName("uuid")]
    [MaxLength(256)]
    public required string Uuid { get; init; }

    /// <summary>
    ///     If this user is approved?
    /// </summary>
    [JsonPropertyName("approved")]
    public bool Approved { get; init; }
}