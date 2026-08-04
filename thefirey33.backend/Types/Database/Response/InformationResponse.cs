using System.Text.Json.Serialization;

namespace thefirey33_backend.Types.Database.Response;

public class InformationResponse
{
    /// <summary>
    ///     Operating System Name.
    /// </summary>
    [JsonPropertyName("os_name")]
    public required string OsName { get; set; }

    /// <summary>
    ///     Operating System Version.
    /// </summary>
    [JsonPropertyName("machine_name")]
    public required string MachineName { get; set; }

    /// <summary>
    ///     The Process Uptime.
    /// </summary>
    [JsonPropertyName("uptime")]
    public required string Uptime { get; set; }
}