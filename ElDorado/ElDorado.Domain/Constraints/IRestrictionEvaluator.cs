using System.Text.Json.Nodes;
using ElDorado.Domain.Constraints.Model;

namespace ElDorado.Domain.Constraints;

public interface IRestrictionEvaluator
{
    List<ConstraintResult> EvaluateRestrictions(JsonObject targetObject, List<Constraint> constraints);
}
