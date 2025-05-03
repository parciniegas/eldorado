
using Common.Operations;

namespace Worker.Api.Services;

public class OperationsService(ILogger<OperationsService> logger, IOperationsRepository repository) : IOperationsService
{
    #region Public methods
    public Operation ChangeStatus(Operation operation)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        operation.SetNextStatus();
        logger.LogInformation("Processing operation {id}, {details}", operation.Id, operation.Details);
        var result = PerformCpuIntensiveTask();

        stopwatch.Stop();
        logger.LogInformation("Elapsed Time: {milliseconds} ms with result {result}", 
            stopwatch.ElapsedMilliseconds, result);
        repository.Add(operation);

        return operation;
    }
    #endregion

    #region Private Methods
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
    #endregion
}
