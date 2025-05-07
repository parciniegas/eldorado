using Common.Constraints;
using ElDorado.Constraints.Domain.Contracts;
using ElDorado.Infrastructure.Persistence.Model;
using FluentResults;
using Microsoft.EntityFrameworkCore;

namespace ElDorado.Infrastructure.Persistence;

public class ConstraintRepository(ElDoradoDbContext context)
    : IConstraintRepository
{
    private readonly ElDoradoDbContext _context = context;

    public async Task<Result<string>> AddConstraintAsync(Constraint constraint)
    {
        try
        {
            var dbConstraint = DbConstraint.FromDomain(constraint);
            dbConstraint.CreateAt = DateTime.UtcNow;
            _context.Constraints.Add(dbConstraint);
            await _context.SaveChangesAsync();
            return Result.Ok(dbConstraint.Id);
        }
        catch (Exception e)
        {
            return Result.Fail(e.Message);
        }
    }

    public async Task<Result<Constraint>> GetConstraintAsync(string id)
    {
        var dbConstraint = await _context.Constraints.FindAsync(id);
        if (dbConstraint is null)
            return Result.Fail<Constraint>($"Constraint with id {id} not found");
        return Result.Ok(DbConstraint.ToDomain(dbConstraint));
    }

    public async Task<Result<List<Constraint>>> GetAllConstraintsAsync()
    {
        try
        {
            var dbConstraints = await _context.Constraints.Include(c => c.Conditions).ToListAsync();
            var constraints = dbConstraints.Select(DbConstraint.ToDomain).ToList();
            return Result.Ok(constraints);
        }
        catch (Exception e)
        {
            return Result.Fail<List<Constraint>>(e.Message);
        }
    }

    public async Task<Result> RemoveConstraintAsync(string id)
    {
        var dbConstraint = await _context.Constraints.Include(c => c.Conditions)
            .FirstOrDefaultAsync(c => c.Id == id);
        if (dbConstraint is null)
            return Result.Fail($"Constraint with id {id} not found");

        try
        {
            _context.Conditions.RemoveRange(dbConstraint.Conditions);
            _context.Constraints.Remove(dbConstraint);
            await _context.SaveChangesAsync();
            return Result.Ok();
        }
        catch (Exception e)
        {
            return Result.Fail(e.Message);
        }
    }

    public async Task<int> GetPendingConstraintsAsync(List<string> ids)
    {
        var count = _context.Constraints
            .AsNoTracking()
            .Count(c => ids.Contains(c.Id));

        return await Task.FromResult(count);
    }
}