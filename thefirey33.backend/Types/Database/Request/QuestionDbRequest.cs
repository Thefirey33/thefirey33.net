using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace thefirey33_backend.Types.Database.Request;

public class QuestionDbRequest
{
    /// <summary>
    ///     The avatar of the author.
    /// </summary>
    [JsonPropertyName("author_id")]
    public ulong UserId { get; init; }

    /// <summary>
    ///     The name of the author of this question.
    /// </summary>
    [JsonPropertyName("author")]
    public required string AuthorName { get; init; }

    /// <summary>
    ///     The question that the user asks.
    /// </summary>
    [MaxLength(1024)]
    [JsonPropertyName("question")]
    public required string Question { get; init; }
}