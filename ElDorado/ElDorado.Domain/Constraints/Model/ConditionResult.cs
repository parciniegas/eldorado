namespace ElDorado.Domain.Constraints.Model;

public class ConditionResult
{
    public string PropertyPath { get; set; }
    public bool WasEvaluated { get; set; }
    public bool IsMet { get; set; }
}
