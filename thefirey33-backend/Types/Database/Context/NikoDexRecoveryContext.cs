using Microsoft.EntityFrameworkCore;

namespace thefirey33_backend.Types.Database.Context;

public class NikoDexRecoveryContext : DbContext
{
    public NikoDexRecoveryContext(DbContextOptions<NikoDexRecoveryContext> options) : base(options)
    {
    }

    /// <summary>
    ///     All the backups of the NikoDex stored in the local database.
    /// </summary>
    public DbSet<NikoDexRecoveryDbType> NikoDexRecovery { get; set; }
}