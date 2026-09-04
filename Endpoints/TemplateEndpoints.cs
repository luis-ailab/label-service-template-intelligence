using Label.Service.TemplateIntelligence.Models;
using Label.Service.TemplateIntelligence.Services;

namespace Label.Service.TemplateIntelligence.Endpoints;

public static class TemplateEndpoints
{
    public static IEndpointRouteBuilder MapTemplateEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/templates").WithTags("Templates");
        group.MapGet("/", (ITemplateCatalog catalog) => Results.Ok(catalog.GetAll()));
        group.MapGet("/{id}", (string id, ITemplateCatalog catalog) =>
            catalog.GetById(id) is { } template ? Results.Ok(template) : Results.NotFound());
        group.MapPost("/recommend", Recommend);
        return app;
    }

    private static IResult Recommend(TemplateRecommendationRequest request,
        ITemplateRecommendationService service)
    {
        try { return Results.Ok(service.Recommend(request)); }
        catch (ArgumentException ex) { return Results.ValidationProblem(new Dictionary<string, string[]> { ["request"] = [ex.Message] }); }
    }
}
