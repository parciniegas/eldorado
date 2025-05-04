namespace ElDorado.Infrastructure.MessageBroker;

public class ConstraintRemovedEvent
{
    public required string ConstraintId { get; set; }
    public DateTime RemovedAt { get; set; }
}
