using ElDorado.Constraints.Domain.Constraints.Model;

namespace ElDorado.Constraints.Api.Endpoints.Add;

public record AddConstraintRequest(string Id, List<Condition> Conditions)
{
    public Constraint ToConstraint()
    {
        return new Constraint(Id, Conditions);
    }
}