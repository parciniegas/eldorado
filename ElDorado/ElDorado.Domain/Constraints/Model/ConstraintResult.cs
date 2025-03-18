namespace ElDorado.Domain.Constraints.Model;

public class ConstraintResult
{
    public string ConstraintId { get; set; }
    public bool IsApplicable { get; set; }
    public List<ConditionResult> EvaluatedConditions { get; set; }
}
