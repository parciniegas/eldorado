namespace Common.Constraints;

public class Constraint(string id, List<Condition> conditions, DateTime? createAt = null)
{
    public string Id { get; set; } = id;
    public DateTime? CreateAt { get; set; } = createAt;
    public List<Condition> Conditions { get; set; } = conditions;
}