using Common.Operations;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class OperationsContext(DbContextOptions<OperationsContext> options) : DbContext(options)
{
    public DbSet<Operation> Operations { get; set; } = null!;
}