using System.Dynamic;
using ElDorado.Domain.Constraints;
using Ilse.MinimalApi.EndPoints;
using Microsoft.AspNetCore.Mvc;

namespace ElDorado.Api.Endpoints.Constraints.Get;

public class GetConstraintsHandler: IEndpoint
{
    public RouteHandlerBuilder Configure(IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapGet("constraints", HandleAsync);
    }

    private static async Task<List<string>> HandleAsync([FromBody]ExpandoObject req, CancellationToken ct)
    {
        var constraints = ConstraintManager.GetConstraints(req);
        var ids = constraints.Select(c => c.Id.ToString()).ToList();

        return await Task.FromResult(ids);
    }
}
