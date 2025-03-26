namespace ElDorado.Infrastructure.MessageBroker;

public class ConstraintRemovedEvent
{
    public string ConstraintId { get; set; }
    public DateTime RemovedAt { get; set; }
}
