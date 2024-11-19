using BackEnd.Entities;
using BackEnd.Models.CustomerModels;
using BackEnd.Models.OutputModels;
using BackEnd.Models.RealEstatePropertyModels;

namespace BackEnd.Interfaces.IBusinessServices
{
    public interface IRealEstatePropertyServices
    {
        Task<RealEstatePropertySelectModel> Create(RealEstatePropertyCreateModel dto);
        Task<ListViewModel<RealEstatePropertySelectModel>> Get(int currentPage, string? filterRequest, char? fromName, char? toName);
        Task<RealEstatePropertySelectModel> Update(RealEstatePropertyUpdateModel dto);
        Task<RealEstatePropertySelectModel> GetById(int id);
        Task<RealEstateProperty> Delete(int id);
    }
}
