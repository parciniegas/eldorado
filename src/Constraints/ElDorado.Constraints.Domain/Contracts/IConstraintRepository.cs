using ElDorado.Constraints.Domain.Constraints.Model;
using FluentResults;

namespace ElDorado.Constraints.Domain.Contracts;

public interface IConstraintRepository
{
    Task<Result> AddConstraintAsync(Constraint constraint);
    Task<Result<Constraint>> GetConstraintAsync(string id);
    Task<Result<List<Constraint>>> GetAllConstraintsAsync();
    Task<Result> RemoveConstraintAsync(string id);
}