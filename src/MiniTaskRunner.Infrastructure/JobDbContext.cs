using Microsoft.EntityFrameworkCore;
using MiniTaskRunner.Core.Domain;

namespace MiniTaskRunner.Infrastructure.Persistence;

public class Class1
{

}

public class JobDbContext : DbContext
{
    public JobDbContext(DbContextOptions<JobDbContext> options) : base(options) { }

    //public DbSet<Job> Jobs => Set<Job>();

    public DbSet<Job> Jobs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Job>(entity =>
        {
            entity.HasKey(j => j.Id);
            entity.Property(j => j.Type).IsRequired();
            entity.Property(j => j.Payload).IsRequired();
            entity.Property(j => j.Status).HasConversion<int>();
        });
    }
}
