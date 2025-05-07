using System.Text.Json;
using System.Text.Json.Nodes;
using Common.Constraints;
using ElDorado.Constraints.Domain.Constraints.Model;
using ElDorado.Constraints.Domain.Contracts;
using ElDorado.Constraints.Domain.Model;
using FluentResults;

namespace ElDorado.Constraints.Domain.Implementations;

public class ConstraintManager(
    IConstraintRepository constraintRepository,
    IConstraintRemovedPublisher constraintRemovedPublisher) : IConstraintManager
{
    private readonly IConstraintRepository _constraintRepository = constraintRepository;

    public async Task<Result<string>> AddConstraintAsync(Constraint constraint)
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
    
    public async Task<Result<int>> GetPendingConstraintsAsync(List<string> ids)
    {
        try
        {
            var count = await _constraintRepository.GetPendingConstraintsAsync(ids);
            return Result.Ok(count);
        }
        catch (Exception ex)
        {
            return await Task.FromResult(Result.Fail<int>(ex.Message));
        }
    }
    public async Task<Result<Constraint>> GetConstraintAsync(string id)
    {
        var result = await _constraintRepository.GetConstraintAsync(id);
        if (result.IsFailed)
            return Result.Fail<Constraint>(result.Errors);
        return Result.Ok(result.Value);
    }

    private static ConditionResult EvaluateCondition(JsonObject entity, Condition condition)
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
            switch (condition.Operator)
            {
                case ConditionOperator.Equals:
                    if (value.GetValueKind() == JsonValueKind.String)
                        conditionResult.IsMet = value.GetValue<string>().Equals(condition.Value.ToString(), StringComparison.OrdinalIgnoreCase);
                    else if (value.GetValueKind() == JsonValueKind.Number)
                        conditionResult.IsMet = value.GetValue<double>() == double.Parse(condition.Value.ToString()!);
                    break;
                case ConditionOperator.GreaterThan:
                    conditionResult.IsMet = value.GetValue<double>() > double.Parse(condition.Value.ToString()!);
                    break;
                case ConditionOperator.LessThan:
                    conditionResult.IsMet = value.GetValue<double>() < (condition.Value as double?);
                    break;
                case ConditionOperator.GreaterThanOrEqual:
                    conditionResult.IsMet = value.GetValue<double>() >= double.Parse(condition.Value.ToString()!);
                    break;
                case ConditionOperator.LessThanOrEqual:
                    conditionResult.IsMet = value.GetValue<double>() <= double.Parse(condition.Value.ToString()!);
                    break;
                default:
                    conditionResult.IsMet = false;
                    break;
            }
    
        }

        return conditionResult;
    }

}