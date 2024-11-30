using BackEnd.Models.OutputModels;

namespace BackEnd.Interfaces
{
    public interface IGenericService
    {
        Task<HomeDetailsModel> GetHomeDetails();
    }
}
