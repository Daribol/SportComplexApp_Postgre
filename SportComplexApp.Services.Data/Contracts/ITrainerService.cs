using Microsoft.AspNetCore.Mvc.Rendering;
using SportComplexApp.Data.Models;
using SportComplexApp.Web.ViewModels.Home;
using SportComplexApp.Web.ViewModels.Trainer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportComplexApp.Services.Data.Contracts
{
    public interface ITrainerService
    {
        //Client-facing
        Task<IEnumerable<AllTrainersViewModel>> GetAllAsync(string? query = null, string? sortBy = null);
        Task<IEnumerable<TrainerHomeViewModel>> GetAllForHomeAsync();
        Task<IEnumerable<AllTrainersViewModel>> GetAllPublicAsync(string? query, int? sportId, string? sortBy = null);

        Task<TrainerDetailsViewModel> GetTrainerDetailsAsync(int trainerId);
        Task<int?> GetTrainerIdByUserId(string userId);
        Task<IEnumerable<AllTrainersViewModel>> GetTrainersBySportIdAsync(int sportId);
        Task<List<TrainerReservationViewModel>> GetReservationsForTrainerAsync(int trainerId);

        //Admin CRUD operations
        Task AddAsync(AddTrainerViewModel model);
        Task<AddTrainerViewModel> GetAddTrainerFormModelAsync();
        Task<IEnumerable<SelectListItem>> GetSportsAsSelectListAsync();
        Task EditAsync(int id, AddTrainerViewModel model);
        Task DeleteAsync(int id);

        Task<AddTrainerViewModel> GetTrainerForEditAsync(int id);
        Task<DeleteTrainerViewModel?> GetTrainerForDeleteAsync(int id);
    }
}
