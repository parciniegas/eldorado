using Common.Operations;
using StackExchange.Redis;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Repositories;

public class RedisRepository : IOperationsRepository
{
    private readonly string _connectionString;
    private readonly ConnectionMultiplexer _redis;
    private readonly IDatabase _redisDatabase;

    public RedisRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("RedisConnection") 
            ?? throw new InvalidOperationException("Redis connection string is not configured.");
        _redis = ConnectionMultiplexer.Connect(_connectionString);
        _redisDatabase = _redis.GetDatabase();
    }

    public void Add(Operation operation)
    {
        var key = $"Operation:{operation.Id}:{operation.Status}";
        var value = System.Text.Json.JsonSerializer.Serialize(operation);
        _redisDatabase.StringSet(key, value);
    }

    public Operation GetById(string id)
    {
        var key = $"Operation:{id}:*";
        var value = _redisDatabase.StringGet(key);
        if (!value.HasValue)
        {
            throw new InvalidOperationException($"Operation with ID {id} was not found.");
        }
        return System.Text.Json.JsonSerializer.Deserialize<Operation>(value!)
            ?? throw new InvalidOperationException($"Deserialization of Operation failed.");
    }

    public List<Operation> GetAll(string? key = null)
    {   
        // Redis does not support querying all entities directly.
        // You would typically use a pattern like "Operation:*" to get all keys
        // and then retrieve them one
        // by one. This is not efficient for large datasets.
        // You can use a Redis scan operation to get all keys matching a pattern.
        // However, this is not a recommended practice for production code.
        // Instead, consider using a more structured approach to store and retrieve your data.
        // For example, you can use a sorted set or a hash to store operations
        // and then query them based on their properties.
        // This is a placeholder implementation and should be replaced with a more efficient approach.
        var server = _redis.GetServer(_connectionString);
        var keys = server.Keys(database: _redisDatabase.Database, pattern: "Operation:*");
        var operations = new List<Operation>();
        foreach (var k in keys)
        {
            var value = _redisDatabase.StringGet(k);   
            if (value.HasValue)
            {
                var operation = System.Text.Json.JsonSerializer.Deserialize<Operation>(value!);
                if (operation != null)
                {
                    operations.Add(operation);
                }
            }
        }
        return operations;
    }

    public void Remove(string id)
    {
        var key = $"Operation:{id}";
        _redisDatabase.KeyDelete(key);
    }

    public void RemoveCollection(string key)
    {
        var server = _redis.GetServer("localhost");
        var keys = server.Keys(pattern: $"Operation:{key}:*");
        foreach (var k in keys)
        {
            _redisDatabase.KeyDelete(k);
        }
    }
}
