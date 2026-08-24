using Microsoft.EntityFrameworkCore;

namespace RifeOS.Apps.Tasks.Data;

public sealed class TasksDbContext : DbContext
{
    private readonly string _dbPath;

    public DbSet<TaskItem> Tasks => Set<TaskItem>();

    public TasksDbContext(string dbPath)
    {
        _dbPath = dbPath;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite($"Data Source={_dbPath}");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TaskItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Priority).HasDefaultValue(1);
            entity.Property(e => e.IsCompleted).HasDefaultValue(false);
        });
    }
}
