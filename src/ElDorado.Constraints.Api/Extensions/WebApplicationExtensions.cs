using ElDorado.Constraints.Api.Endpoints.Add;
using ElDorado.Constraints.Api.Endpoints.Evaluate;
using ElDorado.Constraints.Api.Endpoints.Remove;

namespace ElDorado.Constraints.Api.Extensions;

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