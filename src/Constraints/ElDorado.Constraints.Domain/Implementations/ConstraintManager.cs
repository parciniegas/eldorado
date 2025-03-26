using System.Text.Json.Nodes;
using ElDorado.Constraints.Domain.Constraints.Model;
using ElDorado.Constraints.Domain.Contracts;

namespace ElDorado.Constraints.Domain.Implementations;

public class ConstraintManager(
    IConstraintRepository constraintRepository,
    IConstraintRemovedPublisher constraintRemovedPublisher) : IConstraintManager
{
    private readonly IConstraintRepository _constraintRepository = constraintRepository;

    public async Task AddConstraintAsync(Constraint constraint)
    {
        await _constraintRepository.AddConstraintAsync(constraint);
    }

    public async Task<List<ConstraintResult>> EvaluateConstraintsAsync(JsonObject entity)
    {
        var results = new List<ConstraintResult>();
        var constraints = await _constraintRepository.GetAllConstraintsAsync();
        foreach (var constraint in constraints)
        {
            var constraintResult = new ConstraintResult
            {
                ConstraintId = constraint.Id,
                IsApplicable = true,
                EvaluatedConditions = new List<ConditionResult>()
            };

            foreach (var condition in constraint.Conditions)
            {
                var conditionResult = EvaluateCondition(entity, condition);
                constraintResult.EvaluatedConditions.Add(conditionResult);

                if (conditionResult is { WasEvaluated: true, IsMet: true })
                    continue;

                constraintResult.IsApplicable = false;
                break;
            }

            results.Add(constraintResult);
        }

        return results;
    }

    public async Task RemoveConstraintAsync(string id)
    {
        var removed = await _constraintRepository.RemoveConstraintAsync(id);
        if (removed)
            await constraintRemovedPublisher.PublishAsync(id);
    }

    private ConditionResult EvaluateCondition(JsonObject entity, Condition condition)
    {
        var conditionResult = new ConditionResult
        {
            PropertyPath = condition.PropertyPath,
            WasEvaluated = true,
            IsMet = true
        };

        if (!entity.TryGetPropertyValue(condition.PropertyPath, out var node))
        {
            conditionResult.WasEvaluated = false;
            conditionResult.IsMet = false;
            return conditionResult;
        }

        if (node is not JsonValue value)
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

        return conditionResult;
    }
}