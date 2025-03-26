namespace ElDorado.Constraints.Domain.Constraints.Model;

public class Condition(string propertyPath, string @operator, object value)
{
    public string PropertyPath { get; set; } = propertyPath;
    public string Operator { get; set; } = @operator;
    public object Value { get; set; } = value;
}