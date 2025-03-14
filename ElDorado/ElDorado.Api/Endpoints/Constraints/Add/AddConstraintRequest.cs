using ElDorado.Domain.Constraints;

namespace ElDorado.Api.Endpoints.Constraints.Add;

public class AddConstraintRequest
{
    public Condition[] Conditions { get; set; } = Array.Empty<Condition>();
}
