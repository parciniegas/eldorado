using Operations.Shared;

namespace Operations.Api.Services;

public interface IOperationsService
{
    Operation ProcessOperationAsync(Operation operation);
    Operation CloseOperationAsync(Operation operation, CancellationToken cancellationToken);
}
