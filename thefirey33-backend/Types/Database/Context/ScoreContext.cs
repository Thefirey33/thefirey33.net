using Microsoft.EntityFrameworkCore;

namespace thefirey33_backend.Types.Database.Context;

public class ScoreContext : DbContext
{
    public ScoreContext(DbContextOptions<ScoreContext> options) : base(options)
    {
    }

    /// <summary>
    ///     The scores of each user.
    /// </summary>
    public DbSet<ScoreDbType> Scores { get; set; }
}