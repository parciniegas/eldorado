using System.Text.Json.Nodes;
using Common.Constraints;
using ElDorado.Constraints.Domain.Constraints.Model;

namespace ElDorado.Constraints.Domain.Contracts;

public interface IConstraintEvaluator
{
    List<ConstraintResult> EvaluateRestrictions(JsonObject targetObject, List<Constraint> constraints);
}
