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

    private static ConditionResult EvaluateCondition(JsonObject targetObject, Condition condition)
    {
        var conditionResult = new ConditionResult
        {
            PropertyPath = condition.PropertyPath,
            WasEvaluated = true,
            IsMet = true
        };

        var properties = condition.PropertyPath.Split('.');
        var currentObject = targetObject;

        foreach (var property in properties)
        {
            if (!currentObject.TryGetPropertyValue(property, out var propertyValue))
            {
                conditionResult.WasEvaluated = false;
                conditionResult.IsMet = false;
                return conditionResult;
            }

            if (propertyValue is JsonObject propertyObject)
            {
                currentObject = propertyObject;
                continue;
            }

            if (propertyValue is not JsonValue value)
            {
                conditionResult.WasEvaluated = false;
                conditionResult.IsMet = false;
                return conditionResult;
            }

            conditionResult.IsMet = condition.Operator switch
            {
                ConditionOperator.Equals => value.GetValue<string>()
                    .Equals(condition.Value.ToString(), StringComparison.OrdinalIgnoreCase),
                ConditionOperator.GreaterThan => value.GetValue<double>() > double.Parse(condition.Value.ToString()),
                ConditionOperator.LessThan => value.GetValue<double>() < (condition.Value as double?),
                _ => false
            };

            // if (propertyValue!.ToString() != condition.Value.ToString())
            //     return new ConditionResult
            //     {
            //         PropertyPath = condition.PropertyPath,
            //         WasEvaluated = true,
            //         IsMet = false
            //     };
        }

        return new ConditionResult
        {
            PropertyPath = condition.PropertyPath,
            WasEvaluated = true,
            IsMet = true
        };
    }
}