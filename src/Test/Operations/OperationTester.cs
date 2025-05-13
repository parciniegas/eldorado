using System.Collections.Concurrent;
using Common.Operations;
using Test.Utils;

namespace Test.Operations;

public class OperationTester
{
    #region Fields
    private static readonly int _warmUpCount = 5;
    private static readonly int _operationCount = 500;
    private static readonly int _taskCount = 10;
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
        var id = Guid.NewGuid();
        var operation = new Operation
        {
            Id = id.ToString(),
            Name = $"Operation: {id}",
            Status = OperationStatus.Requested,
            ProductNumber = Random.Shared.Next(1, 10),
        };

        return operation;
    }

    private static ConcurrentQueue<Operation> PopulateQueue(int operationCount)
    {
        Console.WriteLine($"Populating queue with {operationCount} operations...");
        var queue = new ConcurrentQueue<Operation>();

        for (int i = _warmUpCount; i < _operationCount; i++)
        {
            queue.Enqueue(GenerateOperation());
        }
        Console.WriteLine($"Queue populated with {queue.Count} operations.");

        return queue;
    }

    private static Operation ProcessOperation(Operation operation)
    {
        Interlocked.Increment(ref counter);
        Console.WriteLine($"Processing operation {counter} of {_operationCount + _warmUpCount}");
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

        for (int i = 0; i < _warmUpCount; i++)
        {
            operations.Add(GenerateOperation());
        }

        operations.ForEach(op =>
        {
            Console.WriteLine($"Warming up operation {op.Id} with status {op.Status} and details {op.Details}");
            ProcessOperation(op);
            Thread.Sleep(Random.Shared.Next(10, 50));
        });
    }
    #endregion
}
