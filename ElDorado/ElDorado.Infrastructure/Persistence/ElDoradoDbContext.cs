using ElDorado.Infrastructure.Persistence.Model;
using Microsoft.EntityFrameworkCore;

namespace ElDorado.Infrastructure.Persistence;

public class ElDoradoDbContext(DbContextOptions<ElDoradoDbContext> options) : DbContext(options)
{
    public DbSet<DbConstraint> Constraints { get; set; }
    public DbSet<DbCondition> Conditions { get; set; }
}
