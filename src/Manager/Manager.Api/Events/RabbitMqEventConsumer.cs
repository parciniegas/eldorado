using System.Collections.Concurrent;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Manager.Api.Events;

public class RabbitMqEventConsumer : IDisposable
{
    private const string ExchangeName = "constraints.events";

    private bool _disposed = false;
    private readonly IConnection _connection;
    private readonly IChannel _channel;
    private readonly string _queueName;
    private readonly CancellationTokenSource _cts = new();
    private readonly int _expectedMessagesCount;

    public RabbitMqEventConsumer(IEnumerable<string> routingKeys)
    {
        _expectedMessagesCount = routingKeys.Count();

        var factory = new ConnectionFactory()
        {
            Uri = new Uri("amqp://admin:RabbitMQ2023!@localhost:5672"),
        };
        _connection = factory.CreateConnectionAsync().Result;
        _channel = _connection.CreateChannelAsync().Result;
        _channel.ExchangeDeclareAsync(ExchangeName, ExchangeType.Topic, durable: true);
        _queueName = _channel.QueueDeclareAsync().Result.QueueName;

        foreach (var key in routingKeys)
        {
            _channel.QueueBindAsync(_queueName, ExchangeName, $"constraint.removed.{key}");
        }
    }

    public async Task<IReadOnlyList<string>> ConsumeEventsAsync(TimeSpan timeout)
    {
        var completionSource = new TaskCompletionSource<bool>();
        var consumer = new AsyncEventingBasicConsumer(_channel);
        ConcurrentBag<string> _receivedMessages = [];

        consumer.ReceivedAsync += (_, ea) =>
        {
            try
            {
                var message = Encoding.UTF8.GetString(ea.Body.ToArray());
                _receivedMessages.Add(message);
                if (_receivedMessages.Count == _expectedMessagesCount)
                {
                    completionSource.TrySetResult(true);
                }
                _channel.BasicAckAsync(ea.DeliveryTag, false);
            }
            catch (Exception)
            {
                _channel.BasicNackAsync(ea.DeliveryTag, false, false);
            }

            return Task.CompletedTask;
        };
        _ = _channel.BasicConsumeAsync(_queueName, true, consumer);
        _cts.CancelAfter(timeout);

        await Task.WhenAny(
            completionSource.Task,
            Task.Delay(Timeout.Infinite, _cts.Token)
        );

        return [.. _receivedMessages];
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed) return;

        if (disposing)
        {
            _channel?.CloseAsync();
            _channel?.Dispose();
            _connection?.CloseAsync();
            _connection?.Dispose();
        }

        _disposed = true;
    }

    ~RabbitMqEventConsumer()
    {
        Dispose(false);
    }
}
