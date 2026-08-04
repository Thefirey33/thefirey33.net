using System.Text.Json.Serialization;

namespace thefirey33_backend.Types;

public class Author
{
    /// <summary>
    ///     The name of the user.
    /// </summary>
    [JsonPropertyName("login")]
    public required string Name { get; set; }

    /// <summary>
    ///     The HTML Url of the user.
    /// </summary>
    [JsonPropertyName("html_url")]
    public required string HtmlUrl { get; set; }

    /// <summary>
    ///     The HTML Url of the user.
    /// </summary>
    [JsonPropertyName("avatar_url")]
    public required string AvatarUrl { get; set; }
}

public class Commit
{
    /// <summary>
    ///     The message of the Commit.
    /// </summary>
    [JsonPropertyName("message")]
    public required string Message { get; set; }
}

public class TenTrillionGitType
{
    /// <summary>
    ///     GitHub SHA.
    /// </summary>
    [JsonPropertyName("sha")]
    public required string Sha { get; set; }

    /// <summary>
    ///     The ID of the GitHub Commit Node.
    /// </summary>
    [JsonPropertyName("node_id")]
    public required string NodeId { get; set; }

    /// <summary>
    ///     The URL of the Commit.
    /// </summary>
    [JsonPropertyName("html_url")]
    public required string HtmlUrl { get; set; }

    /// <summary>
    ///     The author of the commit.
    /// </summary>
    [JsonPropertyName("author")]
    public required Author Author { get; set; }

    /// <summary>
    ///     The current commit.
    /// </summary>
    [JsonPropertyName("commit")]
    public required Commit Commit { get; set; }
}