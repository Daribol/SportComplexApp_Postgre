using Microsoft.AspNetCore.Mvc.Rendering;
using SportComplexApp.Web.ViewModels.Home;
using SportComplexApp.Web.ViewModels.Report;
using SportComplexApp.Web.ViewModels.Sport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportComplexApp.Services.Data.Contracts
{
    public interface ISportService
    {
        Task<IEnumerable<AllSportsViewModel>> GetAllSportsAsync(string? searchQuery = null, int? minDuration = null, int? maxDuration = null, string? sortBy = null, int? trainerId = null);
        Task<IEnumerable<SportHomeViewModel>> GetAllForHomeAsync();
        Task<SportReservationFormViewModel?> GetReservationFormAsync(int sportId, string? userId);
        Task<int> CreateReservationAsync(SportReservationFormViewModel model, string userId);
        Task<IEnumerable<SportReservationViewModel>> GetUserReservationsAsync(string userId);
        Task<bool> ReservationExistsAsync(int reservationId, string userId);
        Task CancelReservationAsync(int reservationId, string userId);
        Task DeleteExpiredReservationsAsync(string userId);

        Task<AddSportViewModel> GetAddFormModelAsync();
        Task<IEnumerable<SelectListItem>> GetFacilitiesSelectListAsync();
        Task AddAsync(AddSportViewModel model);
        Task<bool> ExistsAsync(string name);
        Task<AddSportViewModel?> GetSportForEditAsync(int id);
        Task EditAsync(int id, AddSportViewModel model);
        Task<DeleteSportViewModel?> GetSportForDeleteAsync(int id);
        Task DeleteAsync(int id);
        Task<IEnumerable<SelectListItem>> GetAllAsSelectListAsync();

        Task<IEnumerable<SportReportViewModel>> GetSportReservationsReportAsync(DateTime? startDate = null, DateTime? endDate = null);
    }
}
