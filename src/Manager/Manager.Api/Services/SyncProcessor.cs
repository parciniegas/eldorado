using Common.Constraints;
using Common.Operations;
using Manager.Api.Utils;

namespace Manager.Api.Services;

public class SyncProcessor(ILogger<SyncProcessor> logger, OperationsHttpClient operationsHttpClient, 
    [FromKeyedServices("RedisKey")]IOperationsRepository redisRepository,
    [FromKeyedServices("SqlKey")]IOperationsRepository sqlRepository) : ISyncProcesor
{
    public async Task<Operation> Process(Operation operation)
    {

        if (operation.Status == OperationStatus.Closed)
            Console.WriteLine($"Operation {operation.Id} was finish with status {operation.Status}");

        if (operation.Status == OperationStatus.Failed)
            Console.WriteLine($"Operation {operation.Id} failed with status {operation.Status}");
        
        //var constraint = CreateConstraint(operation);
        while (operation.Status != OperationStatus.Closed && operation.Status != OperationStatus.Failed)
        {
            operation = await operationsHttpClient.SendOperationAsync(operation);
        }

        if (operation.Status == OperationStatus.Closed)
            operation = CloseOperation(operation, CancellationToken.None).Result;

        return operation;
    }

    public Task<Operation> CloseOperation(Operation operation, CancellationToken cancellationToken)
    {
        if (operation.Status != OperationStatus.Completed)
            throw new InvalidOperationException($"Operation {operation.Id} is not completed");

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
                "CheckProductNumber",
                [
                    new Condition("ProductNumber", "==", operation.ProductNumber),
                ]
            );
        return constraint;
    }
}
