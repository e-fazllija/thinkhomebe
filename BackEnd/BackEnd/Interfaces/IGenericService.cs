using BackEnd.Entities;
using BackEnd.Models.OutputModels;
using BackEnd.Models.LocationModels;

namespace BackEnd.Interfaces
{
    public interface IGenericService
    {
        Task<HomeDetailsModel> GetHomeDetails();
        Task<AdminHomeDetailsModel> GetAdminHomeDetails(string agencyId);
        Task<List<LocationSelectModel>> GetLocations();
        Task<DashboardResponse> GetDashboard(ApplicationUser currentUser, string role, string? agencyId);
        Task<DashboardDataResponse> GetDashboardData(ApplicationUser currentUser, string role, string? agencyId, string? period = null);
        Task<CalendarDetails> GetDashboardAppointments(ApplicationUser currentUser, string role, string? agencyId, string? period = null);
    }
}
