namespace ElDorado.Domain.Constraints;

public class Constraint(Condition[] conditions, int value = 0)
{
    public readonly Guid Id = Guid.NewGuid();
    public readonly Condition[] Conditions = conditions;
    public readonly int TimeToLive = value;
}
