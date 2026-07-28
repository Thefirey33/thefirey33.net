using System.Text.Json.Serialization;
using thefirey33_backend.Types.Database.Dex;

namespace thefirey33_backend.Types.Database;

public class NikoDexRecoveryDbType
{
    /// <summary>
    ///     The ID.
    /// </summary>
    [JsonPropertyName("id")]
    public int Id { get; init; }

    /// <summary>
    ///     The Nikos that this backup has.
    /// </summary>
    [JsonPropertyName("nikos")]
    public required List<NikoTypeRecoveryDb> Nikos { get; init; }
}