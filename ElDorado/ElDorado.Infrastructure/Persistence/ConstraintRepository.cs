using ElDorado.Domain.Constraints.Contracts;
using ElDorado.Domain.Constraints.Model;
using ElDorado.Infrastructure.Persistence.Model;
using Microsoft.EntityFrameworkCore;

namespace ElDorado.Infrastructure.Persistence;

public class ConstraintRepository(ElDoradoDbContext context)
    : IConstraintRepository
{
    private readonly ElDoradoDbContext _context = context;

    public async Task AddConstraintAsync(Constraint constraint)
    {
        var dbConstraint = DbConstraint.FromDomain(constraint);
        _context.Constraints.Add(dbConstraint);
        await _context.SaveChangesAsync();
    }

    public async Task<Constraint?> GetConstraintAsync(string id)
    {
        var dbConstraint = await _context.Constraints.FindAsync(id);
        return DbConstraint.ToDomain(dbConstraint);
    }

    public async Task<List<Constraint>> GetAllConstraintsAsync()
    {
        var dbConstraints = await _context.Constraints.Include(c => c.Conditions).ToListAsync();
        return dbConstraints.Select(DbConstraint.ToDomain).ToList();
    }

    public async Task<bool> RemoveConstraintAsync(string id)
    {
        var dbConstraint = await _context.Constraints.Include(c => c.Conditions)
            .FirstOrDefaultAsync(c => c.Id == id);
        if (dbConstraint is null)
            return false;

        _context.Conditions.RemoveRange(dbConstraint.Conditions);
        _context.Constraints.Remove(dbConstraint);
        await _context.SaveChangesAsync();
        return true;
    }
}
