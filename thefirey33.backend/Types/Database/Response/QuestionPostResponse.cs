using System.Text.Json.Serialization;

namespace thefirey33_backend.Types.Database.Response;

public class QuestionPostResponse
{
    /// <summary>
    ///     If thet posting of the question was successful.
    /// </summary>
    [JsonPropertyName("success")]
    public bool Success { get; set; }

    /// <summary>
    ///     The message provided by the filtering and backend.
    /// </summary>
    [JsonPropertyName("message")]
    public required string Message { get; set; }
}