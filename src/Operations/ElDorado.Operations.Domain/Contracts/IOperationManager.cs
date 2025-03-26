using System.Buffers;

namespace ElDorado.Operations.Domain.Contracts;

public interface IOperationManager
{
    Task TransitAsync(Guid operationId, int status);
}
