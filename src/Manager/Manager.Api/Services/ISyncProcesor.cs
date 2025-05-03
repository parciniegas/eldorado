
using Common.Operations;

namespace Manager.Api.Services;

public interface ISyncProcesor
{
    Task<Operation> Process(Operation operation);
    Task<Operation> CloseOperation(Operation operation, CancellationToken cancellationToken);
}
