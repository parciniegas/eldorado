using ElDorado.Constraints.Domain.Constraints.Model;

namespace ElDorado.Constraints.Domain.Model;

public class ConstraintResult
{
    public string ConstraintId { get; set; }
    public bool IsApplicable { get; set; }
    public DateTime CreateAt { get; set; }
    public List<ConditionResult> EvaluatedConditions { get; set; }
}