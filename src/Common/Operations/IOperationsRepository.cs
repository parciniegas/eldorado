using System;

namespace Common.Operations;

public interface IOperationsRepository
{
    void Add(Operation operation);
    List<Operation> GetAll(string? key = null);
    Operation GetById(string id);
    void Remove(string id);
    void RemoveCollection(string key);

}
