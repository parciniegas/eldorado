using System;

namespace ElDorado.Operations.Domain.Model;

public class Operation
{
    public Guid Id { get; set; }
    public required string State { get; set; }
}
