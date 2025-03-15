using System.Dynamic;
using System.Reflection;

namespace ElDorado.Domain.Constraints;

public class ConstraintManager
{
    private static readonly List<Constraint> Constraints = [];

    public static Guid AddConstraint(Condition[] conditions, int value = 0)
    {
        var constraint = new Constraint(conditions, value);
        Constraints.Add(constraint);

        return constraint.Id;
    }

    public static Guid RemoveConstraint(Guid id)
    {
        Constraints.RemoveAll(c => c.Id == id);
        return id;
    }

    public static List<Constraint> GetConstraints(ExpandoObject @object)
    {
        var list = new List<Constraint>();
        foreach (var constraint in Constraints)
        {
            var contains = true;
            foreach (var condition in constraint.Conditions)
            {
                if (!HasProperty(@object, condition.Field) ||
                    GetPropertyValue(@object, condition.Field) != condition.Value)
                {
                    contains = false;
                    continue;
                }
            }
            if (contains)
                list.Add(constraint);
        }

        return list;
    }

    private static bool HasProperty(dynamic obj, string propertyName)
    {
        var expandoDict = (IDictionary<string, object>)obj;
        return expandoDict.ContainsKey(propertyName);
    }

    static string GetPropertyValue(dynamic obj, string propertyName)
    {
        var expandoDict = (IDictionary<string, object>)obj;
        var result = expandoDict[propertyName].ToString();
        return result!;
    }
}
