using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace thefirey33.catpetterzBackend.Types.Database;

public class CatDbType
{
    /// <summary>
    ///     The ID of the cat.
    /// </summary>
    [JsonPropertyName("id")]
    public int Id { get; set; }

    /// <summary>
    ///     The UserId of the owner of this cat.
    /// </summary>
    [JsonPropertyName("owner")]
    [MaxLength(255)]
    public required string OwnerUserId { get; set; }

    /// <summary>
    ///     The name of the cat.
    /// </summary>
    [JsonPropertyName("name")]
    [MaxLength(256)]
    public required string Name { get; set; }

    /// <summary>
    ///     The health of the cat. This the health of the cat.
    ///     Each time the user keeps the cat hungry, this will receive a 10 point penalty!
    /// </summary>
    [JsonPropertyName("health")]
    public double Health { get; set; } = 100;

    /// <summary>
    ///     This is the hunger of the cat.
    ///     The user must feed the specified cat, otherwise if it reaches the maximum of 255, the hunger penalty will be acted!
    /// </summary>
    [JsonPropertyName("hunger")]
    public byte Hunger { get; set; }

    /// <summary>
    ///     This is the thirst of the cat.
    ///     The user must feed the specified cat with the specified resources, otherwise if it reaches the maximum of 255, the
    ///     cat will be instantly gone!
    /// </summary>
    [JsonPropertyName("thirst")]
    public byte Thirst { get; set; }

    /// <summary>
    ///     The path of the image of the cat.
    /// </summary>
    [MaxLength(256)]
    public required string ImagePath { get; set; }

    /// <summary>
    ///     If the cat has unfortunately passed away,
    ///     This flag will be true.
    /// </summary>
    [JsonPropertyName("the_cat_went_on_some_adventures")]
    public bool TheCatWentOnSomeAdventures { get; set; } = false;
}