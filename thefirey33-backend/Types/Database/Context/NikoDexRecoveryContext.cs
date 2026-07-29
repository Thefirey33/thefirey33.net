using Microsoft.EntityFrameworkCore;
using thefirey33_backend.Types.Database.Dex;

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

    /// <summary>
    ///     All the nikos.
    /// </summary>
    public DbSet<NikoTypeRecoveryDb> NikoTypeRecoveryDb { get; set; }

    /// <summary>
    ///     All the abilities.
    /// </summary>
    public DbSet<AbilityTypeRecoveryDb> AbilityTypeRecoveryDb { get; set; }
}