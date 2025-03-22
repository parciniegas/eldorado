namespace ElDorado.Operations.Api.EndPoints;

public class OperationTransitHandler
{
    public static void Map(WebApplication app)
    {
        app.MapPost("/operations/transit", HandleAsync)
            .WithTags("Operations");
    }

    private static Task HandleAsync(HttpContext context)
    {
        throw new NotImplementedException();
    }
}
