using System.Text.Json;
using Label.Service.TemplateIntelligence.Models;
using Label.Service.TemplateIntelligence.Options;
using Microsoft.Extensions.Options;

namespace Label.Service.TemplateIntelligence.Services;

public sealed class JsonTemplateCatalog : ITemplateCatalog
{
    private readonly IReadOnlyList<LabelTemplate> _templates;

    public JsonTemplateCatalog(IOptions<TemplateCatalogOptions> options, IWebHostEnvironment environment)
    {
        var configured = options.Value.FilePath;
        var path = Path.IsPathRooted(configured)
            ? configured
            : Path.Combine(environment.ContentRootPath, configured);
        if (!File.Exists(path)) throw new FileNotFoundException("Template catalog was not found.", path);
        var json = File.ReadAllText(path);
        _templates = JsonSerializer.Deserialize<List<LabelTemplate>>(json,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
            ?? throw new InvalidOperationException("Template catalog is empty or invalid.");
        if (_templates.Count == 0) throw new InvalidOperationException("Template catalog contains no templates.");
        var duplicate = _templates.GroupBy(x => x.Id, StringComparer.OrdinalIgnoreCase).FirstOrDefault(g => g.Count() > 1);
        if (duplicate is not null) throw new InvalidOperationException($"Duplicate template id: {duplicate.Key}");
    }

    public IReadOnlyList<LabelTemplate> GetAll() => _templates;
    public LabelTemplate? GetById(string id) => _templates.FirstOrDefault(x =>
        string.Equals(x.Id, id, StringComparison.OrdinalIgnoreCase));
}
