using Microsoft.EntityFrameworkCore;

namespace thefirey33_backend.Types.Database.Context;

public class ApprovalContext : DbContext
{
    public ApprovalContext(DbContextOptions<ApprovalContext> options) : base(options)
    {
    }

    public DbSet<ApprovalDbType> Approvals { get; set; }
}