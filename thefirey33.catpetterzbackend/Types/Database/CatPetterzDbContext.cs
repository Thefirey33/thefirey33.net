using Microsoft.EntityFrameworkCore;

namespace thefirey33.catpetterzBackend.Types.Database;

public class CatPetterzDbContext(DbContextOptions<CatPetterzDbContext> options) : DbContext(options)
{
    /// <summary>
    ///     All the cats registered in the database.
    /// </summary>
    public DbSet<CatDbType> Cats { get; set; }
}