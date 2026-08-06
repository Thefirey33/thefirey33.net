using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using thefirey33_backend.Types.Database.Request;

namespace thefirey33_backend.Types.Database;

public class QuestionDbType : QuestionDbRequest
{
    /// <summary>
    ///     The ID of the question asked on the website.
    /// </summary>
    [JsonPropertyName("id")]
    public int Id { get; init; }

    /// <summary>
    ///     The post time of the question.
    /// </summary>
    [JsonPropertyName("time")]
    public DateTime QuestionPostTime { get; init; }

    /// <summary>
    ///     The response from Thefirey33.
    /// </summary>
    [MaxLength(1024)]
    public string? Response { get; set; }

    /// <summary>
    ///     The attached image.
    /// </summary>
    [JsonPropertyName("attachment")]
    [MaxLength(256)]
    public string? Attachment { get; set; }
}