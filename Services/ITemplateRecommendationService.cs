using Label.Service.TemplateIntelligence.Models;
namespace Label.Service.TemplateIntelligence.Services;
public interface ITemplateRecommendationService
{
    TemplateRecommendationResponse Recommend(TemplateRecommendationRequest request);
}
