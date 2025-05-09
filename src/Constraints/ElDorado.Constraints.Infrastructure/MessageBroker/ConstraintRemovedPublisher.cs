using System.Text.Json;
using ElDorado.Constraints.Domain.Contracts;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;

namespace ElDorado.Infrastructure.MessageBroker;

public class ConstraintRemovedPublisher : IConstraintRemovedPublisher
{
    private const string ExchangeName = "constraints.events";
    private readonly IChannel _channel;
    private readonly IConnection _connection;

    public ConstraintRemovedPublisher(IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("RabbitMQ")
            ?? throw new InvalidOperationException("RabbitMQ connection string is not configured.");
        var factory = new ConnectionFactory
        {
            Uri = new Uri(connectionString)
        };

        _connection = factory.CreateConnectionAsync().Result;
        _channel = _connection.CreateChannelAsync().Result;
        _channel.ExchangeDeclareAsync(ExchangeName, ExchangeType.Topic, true);
    }

    public async Task PublishAsync(string constraintId)
    {
        var routingKey = $"constraint.removed.{constraintId}";
        var message = new ConstraintRemovedEvent
        {
            ConstraintId = constraintId,
            RemovedAt = DateTime.UtcNow
        };

        var body = JsonSerializer.SerializeToUtf8Bytes(message);
        var basicProperties = new BasicProperties
        {
            ContentType = "text/plain",
            DeliveryMode = DeliveryModes.Persistent
        };
        await _channel.BasicPublishAsync(ExchangeName, routingKey, false,
            basicProperties, body);
    }
}