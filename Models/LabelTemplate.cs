namespace Label.Service.TemplateIntelligence.Models;

public sealed record LabelTemplate(
    string Id,
    string Name,
    int Version,
    string Status,
    IReadOnlyList<string> Markets,
    IReadOnlyList<string> ProductCategories,
    IReadOnlyList<string> DosageForms,
    IReadOnlyList<string> PackageTypes,
    IReadOnlyList<TemplateSection> Sections,
    IReadOnlyList<string> ContentRules,
    IReadOnlyList<string> Tags);

public sealed record TemplateSection(
    string Key,
    string DisplayName,
    bool Required,
    int Order,
    string Region,
    IReadOnlyList<string> Rules);
