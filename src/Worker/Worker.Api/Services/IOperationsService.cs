using Common.Operations;

namespace Worker.Api.Services
{
    public interface IOperationsService
    {
        Operation ChangeStatus(Operation operation);
    }
}
