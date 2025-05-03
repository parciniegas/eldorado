using Common.Operations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Repositories;

public class SqlServerRepository(ILogger<SqlServerRepository> logger, OperationsContext context) : IOperationsRepository
{
    private readonly OperationsContext _context = context;
    private readonly ILogger<SqlServerRepository> _logger = logger;

    public void Add(Operation operation)
    {
        try
        {
            operation.Id = $"{operation.Id}:{operation.Status}";
            _context.Operations.Add(operation);
            _context.SaveChanges();
        }
        catch (DbUpdateConcurrencyException ex)
        {
            _logger.LogError("Concurrency error: {ex}", ex.Message);
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError("Error adding operation: {ex}", ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError("Unexpected error: {ex}", ex.Message);
        }
    }

    public List<Operation> GetAll(string? key = null)
    {
        if (!string.IsNullOrEmpty(key))
        {
            var filteredOperations = _context.Operations
                .AsNoTracking()
                .Where(o => o.Status.ToString() == key)
                .ToList();
            return filteredOperations;
        }

        var operations = _context.Operations.ToList();
        return operations;
    }

    public Operation GetById(string id)
    {
        var operation = _context.Operations
            .AsNoTracking()
            .FirstOrDefault(o => o.Id == id);
        if (operation == null) 
        {
            _logger.LogWarning("Operation with ID {id} not found", id);
            throw new InvalidOperationException($"Operation with ID {id} not found");
        }
        return operation;
    }

    public void Remove(string id)
    {
        throw new NotImplementedException();
    }

    public void RemoveCollection(string key)
    {
        throw new NotImplementedException();
    }
}
