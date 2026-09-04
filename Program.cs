using Label.Service.TemplateIntelligence.Endpoints;
using Label.Service.TemplateIntelligence.Options;
using Label.Service.TemplateIntelligence.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();
builder.Services.AddHealthChecks();
builder.Services.Configure<TemplateCatalogOptions>(
    builder.Configuration.GetSection(TemplateCatalogOptions.SectionName));
builder.Services.AddSingleton<ITemplateCatalog, JsonTemplateCatalog>();
builder.Services.AddSingleton<ITemplateRecommendationService, TemplateRecommendationService>();

builder.Services.AddCors(options => options.AddPolicy("LabelWeb", policy =>
{
    var origins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];
    if (origins.Length > 0) policy.WithOrigins(origins).AllowAnyHeader().AllowAnyMethod();
}));

var app = builder.Build();
app.UseExceptionHandler();
app.UseHttpsRedirection();
app.UseCors("LabelWeb");
if (app.Environment.IsDevelopment()) app.MapOpenApi();
app.MapHealthChecks("/health");
app.MapTemplateEndpoints();
app.Run();

public partial class Program;
