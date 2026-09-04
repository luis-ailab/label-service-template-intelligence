using Label.Service.TemplateIntelligence.Models;

namespace Label.Service.TemplateIntelligence.Services;

public sealed class TemplateRecommendationService(ITemplateCatalog catalog) : ITemplateRecommendationService
{
    private const string AlgorithmVersion = "rules-v1";

    public TemplateRecommendationResponse Recommend(TemplateRecommendationRequest request)
    {
        Validate(request);
        var required = Normalize(request.RegulatoryRequiredSections);
        var requestedTags = Normalize(request.Tags);

        var ranked = catalog.GetAll()
            .Where(t => string.Equals(t.Status, "Active", StringComparison.OrdinalIgnoreCase))
            .Select(t => Score(t, request, required, requestedTags))
            .OrderByDescending(x => x.Score)
            .ThenBy(x => x.TemplateName, StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (ranked.Count == 0) throw new InvalidOperationException("No active label templates are available.");
        var winner = ranked[0];
        var selected = catalog.GetById(winner.TemplateId)!;
        return new(winner, selected, ranked.Take(5).ToList(), DateTimeOffset.UtcNow, AlgorithmVersion);
    }

    private static TemplateCandidate Score(LabelTemplate t, TemplateRecommendationRequest r,
        HashSet<string> required, HashSet<string> requestedTags)
    {
        double score = 0;
        var reasons = new List<string>();
        Match(t.Markets, r.Market, 30, "Market", ref score, reasons);
        Match(t.ProductCategories, r.ProductCategory, 25, "Product category", ref score, reasons);
        Match(t.DosageForms, r.DosageForm, 25, "Dosage form", ref score, reasons);
        Match(t.PackageTypes, r.PackageType, 15, "Package type", ref score, reasons);

        var templateSections = t.Sections.Select(s => s.Key).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var missing = required.Where(x => !templateSections.Contains(x)).Order().ToList();
        if (required.Count == 0) { score += 5; reasons.Add("No additional regulatory sections were requested"); }
        else {
            var coverage = (required.Count - missing.Count) / (double)required.Count;
            score += 5 * coverage;
            reasons.Add($"Covers {required.Count - missing.Count} of {required.Count} required regulatory sections");
            score -= 40 * missing.Count;
        }

        if (requestedTags.Count > 0)
        {
            var matches = t.Tags.Count(x => requestedTags.Contains(x));
            score += Math.Min(5, matches);
            if (matches > 0) reasons.Add($"Matches {matches} requested tag(s)");
        }

        score = Math.Clamp(score, 0, 100);
        return new(t.Id, t.Name, Math.Round(score, 2), Math.Round(score / 100, 4), reasons, missing);
    }

    private static void Match(IReadOnlyList<string> values, string requested, double weight,
        string label, ref double score, List<string> reasons)
    {
        if (values.Any(x => string.Equals(x, requested, StringComparison.OrdinalIgnoreCase)))
        { score += weight; reasons.Add($"{label} matches '{requested}'"); }
    }

    private static HashSet<string> Normalize(IReadOnlyList<string>? values) =>
        (values ?? []).Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => x.Trim()).ToHashSet(StringComparer.OrdinalIgnoreCase);

    private static void Validate(TemplateRecommendationRequest r)
    {
        var missing = new List<string>();
        if (string.IsNullOrWhiteSpace(r.Market)) missing.Add(nameof(r.Market));
        if (string.IsNullOrWhiteSpace(r.ProductCategory)) missing.Add(nameof(r.ProductCategory));
        if (string.IsNullOrWhiteSpace(r.DosageForm)) missing.Add(nameof(r.DosageForm));
        if (string.IsNullOrWhiteSpace(r.PackageType)) missing.Add(nameof(r.PackageType));
        if (missing.Count > 0) throw new ArgumentException($"Required values: {string.Join(", ", missing)}");
    }
}
