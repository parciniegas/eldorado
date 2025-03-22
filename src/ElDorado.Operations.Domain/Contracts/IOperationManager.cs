using System;
using System.Buffers;
using System.Threading.Tasks;

namespace ElDorado.Operations.Domain.Contracts;

public interface IOperationManager
{
    Task TransitionAsync(Guid operationId, OperationStatus status);
}
