namespace ElDorado.Operations.Domain.Model;

public class Operation
{
    public Guid Id { get; set; }
    public required State State { get; set; }
}
