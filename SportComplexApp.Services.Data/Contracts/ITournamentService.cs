using SportComplexApp.Web.ViewModels.Tournament;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportComplexApp.Services.Data.Contracts
{
    public interface ITournamentService
    {
        Task<IEnumerable<TournamentViewModel>> GetAllAsync(string? searchQuery = null, string? sport = null, string? sortBy = null);
        Task<TournamentViewModel?> GetByIdAsync(int id);
        Task RegisterAsync(int tournamentId, string userId);
        Task<bool> UnregisterAsync(int tournamentId, string userId);
        Task<IEnumerable<TournamentViewModel>> GetUserTournamentsAsync(string userId);
        Task<bool> IsUserRegisteredAsync(int tournamentId, string userId);

        Task AddAsync(AddTournamentViewModel model);
        Task<bool> ExistsAsync(string name);
        Task<AddTournamentViewModel?> GetForEditAsync(int id);
        Task EditAsync(int id, AddTournamentViewModel model);
        Task<DeleteTournamentViewModel?> GetForDeleteAsync(int id);
        Task DeleteAsync(int id);
    }
}
