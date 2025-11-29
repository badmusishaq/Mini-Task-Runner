using Microsoft.EntityFrameworkCore;
using MiniTaskRunner.Core.Domain;

namespace MiniTaskRunner.Infrastructure.Persistence;

public class Class1
{

}

public class JobDbContext : DbContext
{
    public JobDbContext(DbContextOptions<JobDbContext> options) : base(options) { }

    public DbSet<Job> Jobs => Set<Job>();
}
