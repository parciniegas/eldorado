using System.Text.Json.Nodes;
using ElDorado.Constraints.Domain.Constraints.Model;
using ElDorado.Constraints.Domain.Contracts;

namespace ElDorado.Constraints.Domain.Implementations;

public class ConstraintEvaluator : IConstraintEvaluator
{
    public List<ConstraintResult> EvaluateRestrictions(JsonObject targetObject, List<Constraint> constraints)
    {
        var results = new List<ConstraintResult>();

        foreach (var restriction in constraints)
        {
            var constraintResult = new ConstraintResult
            {
                ConstraintId = restriction.Id,
                IsApplicable = true,
                EvaluatedConditions = []
            };

            foreach (var condition in restriction.Conditions)
            {
                var conditionResult = EvaluateCondition(targetObject, condition);
                constraintResult.EvaluatedConditions.Add(conditionResult);

                if (conditionResult.WasEvaluated && conditionResult.IsMet)
                    continue;

                constraintResult.IsApplicable = false;
                break;
            }

            results.Add(constraintResult);
        }

        return results;
    }

    private ConditionResult EvaluateCondition(JsonObject targetObject, Condition condition)
    {
        var propertyPath = condition.PropertyPath.Split('.');
        var currentObject = targetObject;

        foreach (var property in propertyPath)
        {
            if (!currentObject.TryGetPropertyValue(property, out var propertyValue))
                return new ConditionResult
                {
                    PropertyPath = condition.PropertyPath,
                    WasEvaluated = false,
                    IsMet = false
                };

            if (propertyValue is JsonObject propertyObject)
            {
                currentObject = propertyObject;
            }
            else
            {
                if (propertyValue!.ToString() != condition.Value.ToString())
                    return new ConditionResult
                    {
                        PropertyPath = condition.PropertyPath,
                        WasEvaluated = true,
                        IsMet = false
                    };
            }
        }

        return new ConditionResult
        {
            PropertyPath = condition.PropertyPath,
            WasEvaluated = true,
            IsMet = true
        };
    }
}