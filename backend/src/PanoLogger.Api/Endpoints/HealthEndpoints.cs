namespace PanoLogger.Api.Endpoints;

public static class HealthEndpoints
{
    public static IEndpointRouteBuilder MapHealthEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/health", () => Results.Ok(new
        {
            status = "ok",
            service = "PanoLogger.Api",
            utc = DateTimeOffset.UtcNow,
        }))
        .WithName("GetHealth")
        .WithTags("System");

        return app;
    }
}
