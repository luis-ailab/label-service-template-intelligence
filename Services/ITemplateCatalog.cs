using Label.Service.TemplateIntelligence.Models;
namespace Label.Service.TemplateIntelligence.Services;
public interface ITemplateCatalog
{
    IReadOnlyList<LabelTemplate> GetAll();
    LabelTemplate? GetById(string id);
}
