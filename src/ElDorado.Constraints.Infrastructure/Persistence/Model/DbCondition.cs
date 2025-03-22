using System.ComponentModel.DataAnnotations;
using ElDorado.Constraints.Domain.Constraints.Model;

namespace ElDorado.Infrastructure.Persistence.Model;

public class DbCondition
{
    public int Id { get; set; }
    [MaxLength(250)] public string PropertyPath { get; set; }
    [MaxLength(10)] public string Operator { get; set; }
    [MaxLength(50)] public string Value { get; set; }

    public DbConstraint Constraint { get; set; }

    public static Condition ToDomain(DbCondition dbCondition)
    {
        return new Condition(dbCondition.PropertyPath, dbCondition.Operator, dbCondition.Value);
    }

    public static DbCondition FromDomain(Condition condition)
    {
        return new DbCondition
        {
            PropertyPath = condition.PropertyPath, Operator = condition.Operator, Value = condition.Value.ToString()
        };
    }
}