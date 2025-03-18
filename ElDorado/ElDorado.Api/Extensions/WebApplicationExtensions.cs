using ElDorado.Api.Endpoints.Constraints.Add;
using ElDorado.Api.Endpoints.Constraints.Evaluate;
using ElDorado.Api.Endpoints.Constraints.Remove;

namespace ElDorado.Api.Extensions;

public static class WebApplicationExtensions
{
    public static void MapEndpoints(this WebApplication app)
    {
        AddConstraintHandler.Map(app);
        AddConstraintsHandler.Map(app);
        EvaluateConstraintsHandler.Map(app);
        RemoveConstraintHandler.Map(app);
        RemoveConstraintsHandler.Map(app);
    }
}
