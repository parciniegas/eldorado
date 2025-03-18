using System;
using ElDorado.Domain.Constraints;
using ElDorado.Domain.Constraints.Model;

namespace ElDorado.Api.Endpoints.Constraints.Add;

public record AddConstraintRequest(string Id, List<Condition> Conditions)
{
    public Constraint ToConstraint() => new(Id, Conditions);
}
