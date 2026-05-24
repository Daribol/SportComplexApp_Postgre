using SportComplexApp.Web.ViewModels.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportComplexApp.Services.Data.Contracts
{
    public interface IUserService
    {
        Task<IEnumerable<AllUsersViewModel>> GetAllUsersAsync();

        Task<bool> UserExistsByIdAsync(string userId);
        Task<string?> GetUserEmailByIdAsync(string userId);
        Task<bool> AssignUserToRoleAsync(string userId, string role);
        Task<bool> RemoveUserRoleAsync(string userId, string role);
        Task<bool> DeleteUserAsync(string userId);
    }
}
