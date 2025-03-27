using System.Text.Json.Nodes;
using ElDorado.Constraints.Domain.Constraints.Model;
using ElDorado.Constraints.Domain.Contracts;
using FluentResults;

namespace ElDorado.Constraints.Domain.Implementations;

public class ConstraintManager(
    IConstraintRepository constraintRepository,
    IConstraintRemovedPublisher constraintRemovedPublisher) : IConstraintManager
{
    private readonly IConstraintRepository _constraintRepository = constraintRepository;

    public async Task<Result> AddConstraintAsync(Constraint constraint)
    {
        return await _constraintRepository.AddConstraintAsync(constraint);
    }

    public async Task<Result<List<ConstraintResult>>> EvaluateConstraintsAsync(JsonObject entity)
    {
        // TODO: Move evaluation logic to a separate class
        var results = new List<ConstraintResult>();
        var result = await _constraintRepository.GetAllConstraintsAsync();
        if (result.IsFailed)
            return Result.Fail<List<ConstraintResult>>(result.Errors);
        foreach (var constraint in result.Value)
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

    public async Task<Result> RemoveConstraintAsync(string id)
    {
        var result = await _constraintRepository.RemoveConstraintAsync(id);
        if (result.IsSuccess)
            await constraintRemovedPublisher.PublishAsync(id);
        return result;
    }

    private ConditionResult EvaluateCondition(JsonObject entity, Condition condition)
    {
        var conditionResult = new ConditionResult
        {
            PropertyPath = condition.PropertyPath,
            WasEvaluated = true,
            IsMet = true
        };

        var properties = condition.PropertyPath.Split('.');
        var currentEntity = entity;

        foreach (var property in properties)
        {
            if (!currentEntity.TryGetPropertyValue(property, out var propertyValue))
            {
                conditionResult.WasEvaluated = false;
                conditionResult.IsMet = false;
                return conditionResult;
            }

            if (propertyValue is JsonObject propertyObject)
            {
                currentEntity = propertyObject;
                continue;
            }

            if (propertyValue is not JsonValue value)
            {
                conditionResult.WasEvaluated = false;
                conditionResult.IsMet = false;
                return conditionResult;
            }

            // TODO: Add support for other datatypes and operators
            // TODO: Add exception handling for parsing
            conditionResult.IsMet = condition.Operator switch
            {
                ConditionOperator.Equals => value.GetValue<string>()
                    .Equals(condition.Value.ToString(), StringComparison.OrdinalIgnoreCase),
                ConditionOperator.GreaterThan => value.GetValue<double>() > double.Parse(condition.Value.ToString()!),
                ConditionOperator.LessThan => value.GetValue<double>() < (condition.Value as double?),
                ConditionOperator.GreaterThanOrEqual => value.GetValue<double>() >= double.Parse(condition.Value.ToString()!),
                ConditionOperator.LessThanOrEqual => value.GetValue<double>() <= double.Parse(condition.Value.ToString()!),
                _ => false
            };
        }

        return conditionResult;
    }
}