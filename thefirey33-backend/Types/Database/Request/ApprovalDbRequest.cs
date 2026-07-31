using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace thefirey33_backend.Types.Database.Request;

public class ApprovalDbRequest
{
    /// <summary>
    ///     The UUID of the Minecraft user.
    /// </summary>
    [JsonPropertyName("uuid")]
    [MaxLength(256)]
    public required string Uuid { get; init; }

    /// <summary>
    ///     The username of the user.
    /// </summary>
    [JsonPropertyName("username")]
    [MaxLength(256)]
    public required string Username { get; init; }

    /// <summary>
    ///     If this user is approved?
    /// </summary>
    [JsonPropertyName("approved")]
    public bool Approved { get; set; }
}