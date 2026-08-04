using System.Text.Json.Serialization;
using thefirey33_backend.Types.Database.Request;

namespace thefirey33_backend.Types.Database;

public class ApprovalDbType : ApprovalDbRequest
{
    /// <summary>
    ///     The ID of this database entry.
    /// </summary>
    [JsonPropertyName("id")]
    public int Id { get; init; }
}