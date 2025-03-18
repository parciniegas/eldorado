namespace ElDorado.Domain.Constraints.Contracts;

public interface IConstraintRemovedPublisher
{
    Task PublishAsync(string constraintId);
}
