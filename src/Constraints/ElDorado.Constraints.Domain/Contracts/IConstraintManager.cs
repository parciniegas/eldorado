using System.Text.Json.Nodes;
using Common.Constraints;
using ElDorado.Constraints.Domain.Constraints.Model;
using FluentResults;

namespace ElDorado.Constraints.Domain.Contracts;

public interface IConstraintManager
{
    Task<Result<string>> AddConstraintAsync(Constraint constraint);
    Task<Result<List<ConstraintResult>>> EvaluateConstraintsAsync(JsonObject entity);
    Task<Result> RemoveConstraintAsync(string id);
}