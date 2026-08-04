using System.ComponentModel.DataAnnotations;

namespace thefirey33_backend.Types.Database;

public class LikesDbType
{
    /// <summary>
    ///     The ID of the like.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    ///     The origin of who liked this post.
    /// </summary>
    [MaxLength(256)]
    public required string Origin { get; set; }
}