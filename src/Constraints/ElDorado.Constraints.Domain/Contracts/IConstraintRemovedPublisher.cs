namespace ElDorado.Constraints.Domain.Contracts;

public interface IConstraintRemovedPublisher
{
    Task PublishAsync(string constraintId);
}