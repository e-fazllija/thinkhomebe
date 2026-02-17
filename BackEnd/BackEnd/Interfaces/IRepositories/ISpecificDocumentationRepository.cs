using BackEnd.Entities;

namespace BackEnd.Interfaces.IRepositories
{
    public interface ISpecificDocumentationRepository : IGenericRepository<SpecificDocumentation>
    {
        Task<List<SpecificDocumentation>> GetByRealEstatePropertyIdAsync(int realEstatePropertyId);
        Task<List<SpecificDocumentation>> GetByRealEstatePropertyIdAndTypeAsync(int realEstatePropertyId, string documentType);
    }
}
