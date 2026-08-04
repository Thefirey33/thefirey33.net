using System.Text.Json.Serialization;

namespace thefirey33_backend.Types.Database.Response;

public class ApprovalDbResponse
{
    /// <summary>
    ///     Are they approved to join the Minecraft Server?
    /// </summary>
    [JsonPropertyName("is_approval")]
    public bool IsApproved { get; set; }
}