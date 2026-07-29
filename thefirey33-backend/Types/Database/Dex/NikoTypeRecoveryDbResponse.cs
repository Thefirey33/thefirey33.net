using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace thefirey33_backend.Types.Database.Dex;

public class AbilityTypeRecoveryDb
{
    /// <summary>
    ///     The ID of the ability.
    /// </summary>
    [JsonPropertyName("id")]
    public int Id { get; init; }

    /// <summary>
    ///     The ability name.
    /// </summary>
    [JsonPropertyName("name")]
    [MaxLength(256)]
    public required string Name { get; init; }
}

public class NikoTypeRecoveryDbResponse
{
    /// <summary>
    ///     The ID of the Niko.
    /// </summary>
    [JsonPropertyName("id")]
    public int Id { get; init; }

    /// <summary>
    ///     The name of the Niko.
    /// </summary>
    [JsonPropertyName("name")]
    [MaxLength(256)]
    public required string Name { get; init; }

    /// <summary>
    ///     The abilities of this Nikosona.
    /// </summary>
    [JsonPropertyName("abilities")]
    public List<AbilityTypeRecoveryDb>? Abilities { get; init; }

    /// <summary>
    ///     The name of the Author of the Niko. Might be null.
    /// </summary>
    [JsonPropertyName("author_name")]
    [MaxLength(256)]
    public string? AuthorName { get; init; }

    /// <summary>
    ///     The full description of this Nikosona.
    /// </summary>
    [JsonPropertyName("full_desc")]
    [MaxLength(1024)]
    public required string FullDescription { get; init; }

    /// <summary>
    ///     The top header description of this Nikosona.
    /// </summary>
    [JsonPropertyName("description")]
    [MaxLength(256)]
    public required string Description { get; init; }

    /// <summary>
    ///     Is this Niko blacklisted from getting patted?
    /// </summary>
    [JsonPropertyName("is_blacklisted")]
    public bool IsBlacklisted { get; init; }

    public static NikoTypeRecoveryDbResponse FromRecoveryDb(NikoTypeRecoveryDb recoveryDb)
    {
        return new NikoTypeRecoveryDbResponse
        {
            Name = recoveryDb.Name,
            Abilities = recoveryDb.Abilities,
            AuthorName = recoveryDb.AuthorName,
            Description = recoveryDb.Description,
            FullDescription = recoveryDb.FullDescription,
            Id = recoveryDb.Id,
            IsBlacklisted = recoveryDb.IsBlacklisted
        };
    }
}

public class NikoTypeRecoveryDb : NikoTypeRecoveryDbResponse
{
    /// <summary>
    ///     The filepath of the image of the Nikosona.
    /// </summary>
    [JsonPropertyName("path")]
    [MaxLength(256)]
    public string? ImagePath { get; set; }
}