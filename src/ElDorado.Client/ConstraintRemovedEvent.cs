namespace ElDorado.Client;

public class ConstraintRemovedEvent
{
    public string ConstraintId { get; set; }
    public DateTime RemovedAt { get; set; }
}
