using Operations.Shared;
using System.Collections.Concurrent;

namespace Operations.Test.Operations;

public class OperationTester
{
    #region Fields
    private static readonly int _warmUpCount = 5;
    private static readonly int _operationCount = 5000;
    private static readonly int _taskCount = 20;
    private static int counter = 0;
    #endregion

    #region Public Methods
    public static void RunLoadTest()
    {
        var operations = PopulateQueue(_operationCount);
        WarmUp();
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        ProcessQueueOperations(operations, _taskCount);
        stopwatch.Stop();
        Console.WriteLine($"{_operationCount} operations processed in {stopwatch.ElapsedMilliseconds} ms using {_taskCount} tasks");
     }
    #endregion

    #region Private Methods
    private static Operation GenerateOperation()
    {
        var id = Guid.NewGuid().ToString();
        var operation = new Operation
        {
            Id = id,
            Name = $"Operation: {id}",
            Status = OperationStatus.Requested
        };
  
        return operation;
    }

    private static ConcurrentQueue<Operation> PopulateQueue(int operationCount)
    {
        Console.WriteLine($"Populating queue with {operationCount} operations...");
        var queue = new ConcurrentQueue<Operation>();
        Enumerable.Range(0, _operationCount)
            .ToList()
            .ForEach(n => queue.Enqueue(GenerateOperation()));

        return queue;
    }

    private static Operation ProcessOperation(Operation operation)
    {
        Interlocked.Increment(ref counter);
        Console.WriteLine($"Processing operation {counter} of {_operationCount+_warmUpCount}");
        var client = new OperationHttpClient(new HttpClient());
        operation = client.SendOperation(operation);

        if (operation.Status == OperationStatus.Closed)
            Console.WriteLine($"Operation {operation.Id} completed with status {operation.Status}");

        if (operation.Status == OperationStatus.Failed)
            Console.WriteLine($"Operation {operation.Id} failed with status {operation.Status}");

        return operation;
    }

    private static void ProcessQueueOperations(ConcurrentQueue<Operation> queue, int taskCount)
    {
        Console.WriteLine($"Processing queue with {queue.Count} operations using {taskCount} tasks...");
        Task[] tasks = new Task[taskCount];

        for (int i = 0; i < taskCount; i++)
        {
            tasks[i] = Task.Run(() =>
            {
                while (queue.TryDequeue(out var operation))
                {
                    operation = ProcessOperation(operation);
                }
            });
        }

        Task.WaitAll(tasks);
    }

    private static void WarmUp()
    {
        Console.WriteLine("Warming up...");
        var operations = new List<Operation>();

        Enumerable.Range(0, _warmUpCount)
            .ToList()
            .ForEach(i => operations.Add(GenerateOperation()));

        operations.ForEach(op => {
            Console.WriteLine($"Warming up operation {op.Id} with status {op.Status} and details {op.Details}");
            ProcessOperation(op);
            Thread.Sleep(Random.Shared.Next(10, 50));
        });
    }
    #endregion
}