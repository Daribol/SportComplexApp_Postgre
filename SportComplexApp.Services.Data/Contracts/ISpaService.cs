using SportComplexApp.Web.ViewModels.Home;
using SportComplexApp.Web.ViewModels.Report;
using SportComplexApp.Web.ViewModels.Spa;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportComplexApp.Services.Data.Contracts
{
    public interface ISpaService
    {
        Task<PaginationSpaServiceViewModel> GetAllSpaServicesPaginationAsync(
            string? searchQuery = null, 
            int? minDuration = null, 
            int? maxDuration = null,
            string? sortBy = null,
            int currentPage = 1,
            int spaPerpage = 9,
            int maxPages = 3);
        Task<IEnumerable<SpaServiceViewModel>> GetAllSpaServicesAsync(string? searchQuery = null, string? sortBy = null);
        Task<IEnumerable<SpaProcedureHomeViewModel>> GetAllForHomeAsync();
        Task<SpaReservationFormViewModel?> GetSpaServiceByIdAsync(int id);
        Task<int> CreateReservationAsync(SpaReservationFormViewModel model, string userId);
        Task<IEnumerable<MySpaReservationViewModel>> GetUserReservationsAsync(string userId);
        Task<SpaDetailsViewModel?> GetSpaDetailsByIdAsync(int id);
        Task CancelReservationAsync(int reservationId, string userId);
        Task<bool> ReservationExistsAsync(int reservationId, string userId);
        Task DeleteExpiredSpaReservationsAsync(string userId);
        Task<int> GetSpaServicesCountAsync(string? searchQuery, int? minDuration, int? maxDuration);

        Task AddAsync(AddSpaServiceViewModel model);
        Task<bool> ExistsAsync(string name);
        Task<AddSpaServiceViewModel?> GetForEditAsync(int id);
        Task EditAsync(int id, AddSpaServiceViewModel model);
        Task<DeleteSpaServiceViewModel?> GetForDeleteAsync(int id);
        Task DeleteAsync(int id);

        Task<IEnumerable<SpaReportViewModel>> GetSpaReservationsReportAsync(DateTime? startDate = null, DateTime? endDate = null);
    }
}
