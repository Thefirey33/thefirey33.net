using System.Text.Json.Serialization;

namespace thefirey33.catpetterzBackend.Types;

public class DiscordAuthLinkResponse
{
    /// <summary>
    ///     The URL of the redirect.
    /// </summary>
    [JsonPropertyName("url")]
    public required string Url { get; set; }
}