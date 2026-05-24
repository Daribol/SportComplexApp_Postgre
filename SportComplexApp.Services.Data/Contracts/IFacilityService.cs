using SportComplexApp.Web.ViewModels.Facility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportComplexApp.Services.Data.Contracts
{
    public interface IFacilityService
    {
        Task<IEnumerable<FacilityMasterViewModel>> GetAllFacilitiesWithSportsAsync();
        Task AddAsync(AddFacilityViewModel model);
        Task<bool> ExistsAsync(string name);
        Task<AddFacilityViewModel?> GetFacilityForEditAsync(int id);
        Task EditAsync(int id, AddFacilityViewModel model);
        Task<DeleteFacilityViewModel?> GetFacilityForDeleteAsync(int id);
        Task DeleteAsync(int id);
    }
}
