using System.Text.Json.Serialization;

namespace thefirey33.catpetterzBackend.Types;

public class DiscordAuthResponse
{
    /// <summary>
    ///     The owner userid.
    /// </summary>
    [JsonPropertyName("id")]
    public required string Id { get; set; }

    /// <summary>
    ///     The username of the user.
    /// </summary>
    [JsonPropertyName("username")]
    public required string Username { get; init; }

    /// <summary>
    ///     The email of the user.
    /// </summary>
    [JsonPropertyName("email")]
    public required string Email { get; init; }

    /// <summary>
    ///     The URL of the avatar of the user.
    /// </summary>
    [JsonPropertyName("avatar_url")]
    public string? AvatarUrl { get; init; }
}