using BackEnd.Entities;
using BackEnd.Models.SpecificDocumentationModels;

namespace BackEnd.Interfaces.IBusinessServices
{
    public interface ISpecificDocumentationServices
    {
        Task<SpecificDocumentationSelectModel> Create(SpecificDocumentationCreateModel dto);
        Task<List<SpecificDocumentationSelectModel>> GetByRealEstatePropertyId(int realEstatePropertyId);
        Task<List<SpecificDocumentationSelectModel>> GetByRealEstatePropertyIdAndType(int realEstatePropertyId, string documentType);
        Task Delete(int id);
    }
}
