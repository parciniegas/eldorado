using Common.Constraints;
using FluentResults;

namespace ElDorado.Constraints.Domain.Contracts;

public interface IConstraintRepository
{
    Task<Result<string>> AddConstraintAsync(Constraint constraint);
    Task<Result<Constraint>> GetConstraintAsync(string id);
    Task<Result<List<Constraint>>> GetAllConstraintsAsync();
    Task<int> GetPendingConstraintsAsync(List<string> ids);
    Task<Result> RemoveConstraintAsync(string id);
}