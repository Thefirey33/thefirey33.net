using Microsoft.EntityFrameworkCore;

namespace thefirey33_backend.Types.Database.Context;

public class ArtsContext : DbContext
{
    public ArtsContext(DbContextOptions<ArtsContext> options)
        : base(options)
    {
    }

    /// <summary>
    ///     The arts referenced.
    /// </summary>
    public DbSet<ArtDbType> Arts { get; set; }
}