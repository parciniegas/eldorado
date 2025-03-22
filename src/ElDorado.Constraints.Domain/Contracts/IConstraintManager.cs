using System.Text.Json.Nodes;
using ElDorado.Constraints.Domain.Constraints.Model;

namespace ElDorado.Constraints.Domain.Contracts;

public interface IConstraintManager
{
    Task AddConstraintAsync(Constraint constraint);
    Task<List<ConstraintResult>> EvaluateConstraintsAsync(JsonObject entity);
    Task RemoveConstraintAsync(string id);
}