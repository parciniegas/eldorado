using ElDorado.Domain.Constraints.Model;

namespace ElDorado.Infrastructure.Persistence.Model;

public class DbConstraint
{
    public string Id { get; set; }
    public List<DbCondition> Conditions { get; set; }

    public static DbConstraint FromDomain(Constraint constraint) =>
        new DbConstraint{Id = constraint.Id, Conditions = constraint.Conditions.Select(DbCondition.FromDomain).ToList()};

    public static Constraint ToDomain(DbConstraint dbConstraint) =>
        new(dbConstraint.Id, dbConstraint.Conditions.Select(DbCondition.ToDomain).ToList());
}
