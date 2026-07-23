using System.Text.Json.Serialization;

namespace thefirey33_backend.Types;

public class UserType
{
    /// <summary>
    /// The username of the user.
    /// </summary>
    [JsonPropertyName("name")]
    public required string Username { get; set; }
    
    /// <summary>
    /// The Password of the user.
    /// </summary>
    [JsonPropertyName("password")]
    public required string Password { get; set; }
}