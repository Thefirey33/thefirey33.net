using System.Text.Json.Serialization;

namespace thefirey33_backend.Types.Database.Dex;

public class NikoRecoveryResponse
{
    /// <summary>
    ///     The amount of pages that the user can scroll through.
    /// </summary>
    [JsonPropertyName("pages")]
    public int AmountPages { get; init; }

    /// <summary>
    ///     The time that the backup was taken.
    /// </summary>
    [JsonPropertyName("date")]
    public DateTime BackupTime { get; init; }
}