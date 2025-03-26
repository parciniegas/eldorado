namespace ElDorado.Operations.Domain.Model;

public class State
{
    public int Value { get; set; }
    public int[]? Next { get; set; }
    public int[]? Previous { get; set; }
}
