using Microsoft.EntityFrameworkCore;
using Operations.Infrastructure.Repositories;
using Operations.Shared;

namespace Operations.Api.Services;

public class OperationsService(ILogger<OperationsService> logger, 
    [FromKeyedServices("redis")]IRepository redisRepository,
    [FromKeyedServices("sql")] IRepository sqlRepository) : IOperationsService
{
    public Operation ProcessOperationAsync(Operation operation)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        operation.SetNextStatus();
        logger.LogInformation("Processing operation {id}, {details}", operation.Id, operation.Details);
        var result = PerformCpuIntensiveTask();

        stopwatch.Stop();
        logger.LogInformation("Elapsed Time: {milliseconds} ms with result {result}", 
            stopwatch.ElapsedMilliseconds, result);
        redisRepository.Add(operation);

        return operation;
    }

    private static int PerformCpuIntensiveTask()
    {
        var delay = new Random().Next(250, 500);
        Thread.Sleep(delay);
        // while (stopwatch.ElapsedMilliseconds < new Random().Next(250, 500))
        // {
        //     // Perform a CPU-intensive operation
        //     result = Math.Sqrt(new Random().NextDouble());
        // }

        return delay;
    }

    public Operation CloseOperationAsync(Operation operation, CancellationToken cancellationToken)
    {
        if (operation.Status != OperationStatus.Completed)
            throw new InvalidOperationException($"Operation {operation.Id} is not completed");

        var operations = redisRepository.GetAll(operation.Id) 
            ?? throw new InvalidOperationException($"Operation {operation.Id} not found");

        foreach (var o in operations)
        {
            logger.LogInformation("Completing operation {id}, {details}", o.Id, o.Details);
            sqlRepository.Add(o);
        }
        
        foreach (var o in operations)
        {
            logger.LogInformation("Deleting operation {id}, {details}", o.Id, o.Details);
            redisRepository.Remove(o.Id);
        }

        operation.SetNextStatus();
        return operation;
    }
}
