using Operations.Shared;

namespace Operations.Infrastructure.Repositories;

public interface IRepository
{
    void Add(Operation operation);
    Operation GetById(string id);
    List<Operation> GetAll(string? key = null);
    void Remove(string id);
    void RemoveCollection(string key);
}
