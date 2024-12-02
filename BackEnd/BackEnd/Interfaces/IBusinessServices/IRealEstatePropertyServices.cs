using BackEnd.Models.OutputModels;
using BackEnd.Models.RealEstatePropertyModels;

namespace BackEnd.Interfaces.IBusinessServices
{
    public interface IRealEstatePropertyServices
    {
        Task<RealEstatePropertySelectModel> Create(RealEstatePropertyCreateModel dto);
        Task InsertFiles(UploadFilesModel dto);
        Task<ListViewModel<RealEstatePropertySelectModel>> Get(int currentPage, string? filterRequest, string? status, string? typologie, char? fromName, char? toName);
        Task<RealEstatePropertyCreateViewModel> GetToInsert();
        Task<RealEstatePropertySelectModel> Update(RealEstatePropertyUpdateModel dto);
        Task<RealEstatePropertySelectModel> GetById(int id);
        Task Delete(int id);
        Task SetHighlighted(int realEstatePropertyId);
        Task SetInHome(int realEstatePropertyId);
    }
}
