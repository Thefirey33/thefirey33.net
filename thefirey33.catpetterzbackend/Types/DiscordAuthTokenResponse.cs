using System.Text.Json.Serialization;

namespace thefirey33.catpetterzBackend.Types;

public class DiscordAuthTokenResponse
{
    /// <summary>
    ///     The Discord Authentication Access Token.
    /// </summary>
    [JsonPropertyName("access_token")]
    public required string AccessToken { get; set; }

    /// <summary>
    ///     The Discord Authentication Refresh Token.
    /// </summary>
    [JsonPropertyName("refresh_token")]
    public required string RefreshToken { get; set; }
}