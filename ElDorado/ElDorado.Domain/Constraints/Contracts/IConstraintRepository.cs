using ElDorado.Domain.Constraints.Model;

namespace ElDorado.Domain.Constraints.Contracts;

public interface IConstraintRepository
{
    Task AddConstraintAsync(Constraint? constraint);
    Task<Constraint?> GetConstraintAsync(string id);
    Task<List<Constraint>> GetAllConstraintsAsync();
    Task<bool> RemoveConstraintAsync(string id);
}
