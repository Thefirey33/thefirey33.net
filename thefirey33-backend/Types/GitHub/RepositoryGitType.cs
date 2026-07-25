using System.Text.Json.Serialization;

namespace thefirey33_backend.Types;

public class RepositoryGitType
{
    /// <summary>
    ///     The ID of this repository.
    /// </summary>
    [JsonPropertyName("id")]
    public int Id { get; set; }

    /// <summary>
    ///     The name of this repository.
    /// </summary>
    [JsonPropertyName("name")]
    public required string Name { get; set; }

    /// <summary>
    ///     The owner of this repository.
    /// </summary>
    [JsonPropertyName("owner")]
    public required Author Owner { get; set; }

    /// <summary>
    ///     The description of this repository.
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>
    ///     The link to the repository.
    /// </summary>
    [JsonPropertyName("html_url")]
    public required string HtmlUrl { get; set; }

    /// <summary>
    ///     When was the repository created at?
    /// </summary>
    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; set; }

    /// <summary>
    ///     The top language of the repository.
    /// </summary>
    [JsonPropertyName("language")]
    public string? Language { get; set; }

    /// <summary>
    ///     Is this repository archived?
    /// </summary>
    [JsonPropertyName("archived")]
    public bool Archived { get; set; }
}