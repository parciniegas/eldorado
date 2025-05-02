using Microsoft.EntityFrameworkCore;
using Operations.Shared;

namespace Operations.Infrastructure.Repositories;

public class OperationsContext(DbContextOptions<OperationsContext> options) : DbContext(options)
{
    public DbSet<Operation> Operations { get; set; } = null!;
}
