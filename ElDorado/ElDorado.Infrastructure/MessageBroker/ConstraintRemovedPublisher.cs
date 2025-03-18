using System.Text.Json;
using ElDorado.Domain.Constraints.Contracts;
using RabbitMQ.Client;

namespace ElDorado.Infrastructure.MessageBroker;

public class ConstraintRemovedPublisher: IConstraintRemovedPublisher
{
    private readonly IConnection _connection;
    private readonly IChannel _channel;
    private const string ExchangeName = "constraints.events";

    public ConstraintRemovedPublisher()
    {
        var factory = new ConnectionFactory( );
        factory.Uri = new Uri( "amqp://admin:P2ssw0rd@localhost:5672" );

        _connection = factory.CreateConnectionAsync().Result;
        _channel = _connection.CreateChannelAsync().Result;
        _channel.ExchangeDeclareAsync(ExchangeName, ExchangeType.Topic, durable: true);
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
        await _channel.BasicPublishAsync<BasicProperties>(exchange: ExchangeName, routingKey: routingKey, mandatory: false,
            basicProperties: basicProperties, body: body);
    }
}
