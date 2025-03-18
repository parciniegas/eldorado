using System.Text.Json.Nodes;
using ElDorado.Domain.Constraints.Model;

namespace ElDorado.Domain.Constraints.Contracts;

public interface IConstraintManager
{
    Task AddConstraintAsync(Constraint constraint);
    Task<List<ConstraintResult>> EvaluateConstraintsAsync(JsonObject entity);
    Task RemoveConstraintAsync(string id);
}
