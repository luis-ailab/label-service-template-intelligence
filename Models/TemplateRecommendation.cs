namespace Label.Service.TemplateIntelligence.Models;

public sealed record TemplateRecommendationRequest(
    string Market,
    string ProductCategory,
    string DosageForm,
    string PackageType,
    IReadOnlyList<string>? RegulatoryRequiredSections = null,
    IReadOnlyList<string>? Tags = null);

public sealed record TemplateCandidate(
    string TemplateId,
    string TemplateName,
    double Score,
    double Confidence,
    IReadOnlyList<string> Reasons,
    IReadOnlyList<string> MissingRequiredSections);

public sealed record TemplateRecommendationResponse(
    TemplateCandidate Selected,
    LabelTemplate Template,
    IReadOnlyList<TemplateCandidate> Candidates,
    DateTimeOffset EvaluatedAtUtc,
    string AlgorithmVersion);
