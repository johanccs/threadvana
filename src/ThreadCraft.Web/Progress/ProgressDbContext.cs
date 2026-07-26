using Microsoft.EntityFrameworkCore;

namespace ThreadCraft.Web.Progress;

/// <summary>
/// EF Core context for the single-learner progress database (threadcraft-progress.db).
/// </summary>
public sealed class ProgressDbContext : DbContext
{
    public ProgressDbContext(DbContextOptions<ProgressDbContext> options)
        : base(options)
    {
    }

    public DbSet<ProgressRecordEntity> ProgressRecords => Set<ProgressRecordEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ProgressRecordEntity>().HasKey(e => e.LessonId);
    }
}