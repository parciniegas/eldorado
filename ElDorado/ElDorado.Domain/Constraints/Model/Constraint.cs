namespace ElDorado.Domain.Constraints.Model;

public class Constraint(string id, List<Condition> conditions)
{
    public string Id { get; set; } = id;
    public List<Condition> Conditions { get; set; } = conditions;
}
