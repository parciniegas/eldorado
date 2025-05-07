using Common.Constraints;
using Common.Operations;
using Manager.Api.Utils;

namespace Manager.Api.Services;

public class SyncProcessor(ILogger<SyncProcessor> logger, 
    OperationsHttpClient operationsHttpClient, 
    ConstraintHttpClient constraintHttpClient,
    [FromKeyedServices("RedisKey")]IOperationsRepository redisRepository,
    [FromKeyedServices("SqlKey")]IOperationsRepository sqlRepository) : ISyncProcesor
{
    public async Task<Operation> Process(Operation operation)
    {
        if (operation.Status == OperationStatus.Failed)
            Console.WriteLine($"Operation {operation.Id} failed with status {operation.Status}");

        var constraint = CreateConstraint(operation);
        var constraintId = await constraintHttpClient.AddConstraint(constraint) 
            ?? throw new InvalidOperationException($"Failed to add constraint {constraint.Id}");
        logger.LogInformation("Constraint {id} added", constraintId);
    
        try
        {
            var appliedConstrains = await constraintHttpClient.GetConstraints(operation, constraintId);
            var count = appliedConstrains.Count;
            while (count > 0) {
                Task.Delay(250).Wait();
                var lastCount = count;
                count = await constraintHttpClient.GetPendingConstraints(appliedConstrains);
                Console.WriteLine($"Pending constraints: last count {lastCount}, current count {count}");
            };
    
            while (operation.Status != OperationStatus.Closed && operation.Status != OperationStatus.Failed)
            {
                operation = await operationsHttpClient.SendOperationAsync(operation);
            }
    
            if (operation.Status == OperationStatus.Closed)
                operation = CloseOperation(operation, CancellationToken.None).Result;
        }
        catch {
            throw;
        }
        finally {
            await constraintHttpClient.RemoveConstraint(constraint.Id);
        }


        return operation;
    }

    public Task<Operation> CloseOperation(Operation operation, CancellationToken cancellationToken)
    {
        if (operation.Status != OperationStatus.Closed)
            throw new InvalidOperationException($"Operation {operation.Id} is not closed");

        var operations = redisRepository.GetAll(operation.Id) 
            ?? throw new InvalidOperationException($"Operation {operation.Id} not found");

        foreach (var o in operations)
        {
            logger.LogInformation("Completing operation {id}, {details}", o.Id, o.Details);
            sqlRepository.Add(o);
            redisRepository.Remove(o.Id);
            logger.LogInformation("Operation {id} completed", o.Id);
        }
        
        return Task.FromResult(operation);
    }

    private Constraint CreateConstraint(Operation operation)
    {
        var constraint
            = new Constraint(
                $"CheckProductNumber:{Guid.NewGuid()}",
                
                [
                    new Condition("ProductNumber", "=", operation.ProductNumber),
                ]
            );
        return constraint;
    }
}
