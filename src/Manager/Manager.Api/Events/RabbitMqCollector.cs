using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Manager.Api.Events;

public class RabbitMqCollector
{
    private readonly IConnection _connection;
    private readonly IChannel _channel;
    private const string ExchangeName = "constraints.events";
    private readonly TaskCompletionSource<bool> _taskCompletionSource = new();
    private int _receivedCount = 0;
    private int _expectedEventCount = 0;
    private readonly object _lock = new();

    public RabbitMqCollector(IEnumerable<string> interestedRestrictionIds)
    {
        var factory = new ConnectionFactory
        {
            Uri = new Uri("amqp://admin:RabbitMQ2023!@localhost:5672")
        };
        _connection = factory.CreateConnectionAsync().Result;
        _channel = _connection.CreateChannelAsync().Result;
        _channel.ExchangeDeclareAsync(ExchangeName, ExchangeType.Topic, durable: true);
        InitConsumer(interestedRestrictionIds);
    }

    private void InitConsumer(IEnumerable<string> interestedRestrictionIds)
    {
        _expectedEventCount = interestedRestrictionIds.Count();
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

            lock (_lock)
            {
                _receivedCount++;
                if (_receivedCount == _expectedEventCount)
                {
                    _taskCompletionSource.SetResult(true);
                }
            }
            ProcessRestrictionDeletion(message);
            await _channel.BasicAckAsync(args.DeliveryTag, false);
        }
        catch (Exception ex)
        {
            await _channel.BasicNackAsync(args.DeliveryTag, false, true);
        }
    }

    public async Task WaitForAllEventsAsync()
    {
        await _taskCompletionSource.Task;
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
