using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace ElDorado.Client;

public class ConstraintRemoveSubscriber : IDisposable, IAsyncDisposable
{
    private readonly IConnection _connection;
    private readonly IChannel _channel;
    private const string ExchangeName = "constraints.events";

    public ConstraintRemoveSubscriber(IEnumerable<string> interestedRestrictionIds)
    {
        var factory = new ConnectionFactory
        {
            Uri = new Uri("amqp://admin:P2ssw0rd@localhost:5672")
        };
        _connection = factory.CreateConnectionAsync().Result;
        _channel = _connection.CreateChannelAsync().Result;
        _channel.ExchangeDeclareAsync(ExchangeName, ExchangeType.Topic, durable: true);
        var interestedRestrictionIds1 = new HashSet<string>(interestedRestrictionIds);

        _channel.ExchangeDeclareAsync(
            exchange: ExchangeName,
            type: ExchangeType.Topic,
            durable: true);

        var queueName = _channel.QueueDeclareAsync(
            queue: "",
            durable: false,
            exclusive: true,
            autoDelete: true).Result.QueueName;

        foreach (var id in interestedRestrictionIds1)
        {
            _channel.QueueBindAsync(
                queue: queueName,
                exchange: ExchangeName,
                routingKey: $"constraint.removed.{id}");
        }

        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += OnMessageReceived;

        _channel.BasicConsumeAsync(
            queue: queueName,
            autoAck: false,
            consumer: consumer);
    }

    private async Task OnMessageReceived(object sender, BasicDeliverEventArgs args)
    {
        try
        {
            var message = JsonSerializer.Deserialize<ConstraintRemovedEvent>(args.Body.Span);

            ProcessRestrictionDeletion(message);
            await _channel.BasicAckAsync(args.DeliveryTag, false);
        }
        catch (Exception ex)
        {
            await _channel.BasicNackAsync(args.DeliveryTag, false, true);
        }
    }

    private static void ProcessRestrictionDeletion(ConstraintRemovedEvent? message)
    {
        Console.WriteLine($"Removing restriction {message?.ConstraintId}");
    }

    public void Dispose()
    {
        _connection.Dispose();
        _channel.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        await _connection.DisposeAsync();
        await _channel.DisposeAsync();
    }
}
