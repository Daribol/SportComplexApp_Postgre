using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using SportComplexApp.Common;
using SportComplexApp.Data.Models;
using SportComplexApp.Services.Data.Contracts;
using SportComplexApp.Web.Controllers;
using SportComplexApp.Web.ViewModels.Admin;
using static SportComplexApp.Common.ErrorMessages.Users;
using static SportComplexApp.Common.SuccessfulValidationMessages.Users;

namespace SportComplexApp.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class UserManagementController : BaseController
    {
        private readonly IUserService userService;
        private readonly UserManager<Client> userManager;
        private readonly IStringLocalizer<SharedResource> sharedLocalizer;

        public UserManagementController(IUserService userService, UserManager<Client> userManager, IStringLocalizer<SharedResource> sharedLocalizer)
        {
            this.userService = userService;
            this.userManager = userManager;
            this.sharedLocalizer = sharedLocalizer;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int page = 1)
        {
            int pageSize = 10;

            var allUsers = await userService.GetAllUsersAsync();

            int totalItems = allUsers.Count();
            int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            if (page < 1) page = 1;
            if (page > totalPages && totalPages > 0) page = totalPages;

            var paginatedUsers = allUsers
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            return View(paginatedUsers);
        }

        [HttpPost]
        public async Task<IActionResult> AssignRole(string userId, string role)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(role))
            {
                TempData["ErrorMessage"] = sharedLocalizer[UserIdOrRoleCannotBeEmpty].Value;
                return RedirectToAction(nameof(Index));
            }

            if (role.Equals("Client", StringComparison.OrdinalIgnoreCase))
            {
                TempData["ErrorMessage"] = "Ролята 'Клиент' е базова и не може да бъде манипулирана ръчно.";
                return RedirectToAction(nameof(Index));
            }

            bool userExists = await userService.UserExistsByIdAsync(userId);

            if (!userExists)
            {
                return NotFound();
            }

            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                TempData["ErrorMessage"] = sharedLocalizer[UserDoesNotExist].Value;
                return RedirectToAction(nameof(Index));
            }

            if (await userManager.IsInRoleAsync(user, role))
            {
                TempData["ErrorMessage"] = sharedLocalizer[UserAlreadyInRole].Value;
                return RedirectToAction(nameof(Index));
            }

            bool assignResult = await userService.AssignUserToRoleAsync(userId, role);

            if (!assignResult)
            {
                TempData["ErrorMessage"] = sharedLocalizer[FailedToAssignRole].Value;
                return RedirectToAction(nameof(Index));
            }

            TempData["SuccessMessage"] = sharedLocalizer[RoleAssigned].Value;
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> RemoveRole(string userId, string role)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(role))
            {
                TempData["ErrorMessage"] = sharedLocalizer[UserIdOrRoleCannotBeEmpty].Value;
                return RedirectToAction(nameof(Index));
            }

            if (role.Equals("Client", StringComparison.OrdinalIgnoreCase))
            {
                TempData["ErrorMessage"] = "Ролята 'Клиент' е базова и не може да бъде премахната.";
                return RedirectToAction(nameof(Index));
            }

            bool userExists = await userService.UserExistsByIdAsync(userId);

            if (!userExists)
            {
                return NotFound();
            }

            bool removeRoleResult = await userService.RemoveUserRoleAsync(userId, role);

            if (!removeRoleResult)
            {
                TempData["ErrorMessage"] = sharedLocalizer[FailedToRemoveRole].Value;
                return RedirectToAction(nameof(Index));
            }

            TempData["SuccessMessage"] = sharedLocalizer[RoleRemoved].Value;
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> DeleteUser(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                TempData["ErrorMessage"] = sharedLocalizer[UserIdOrRoleCannotBeEmpty].Value;
                return RedirectToAction(nameof(Index));
            }

            bool userExists = await userService.UserExistsByIdAsync(id);

            if (!userExists)
            {
                return NotFound();
            }

            string? userEmail = await userService.GetUserEmailByIdAsync(id);

            var model = new DeleteUserViewModel
            {
                Id = id,
                Email = userEmail ?? "Unknown email"
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUserConfirmed(DeleteUserViewModel model)
        {
            if (string.IsNullOrEmpty(model.Id))
            {
                TempData["ErrorMessage"] = sharedLocalizer[UserIdOrRoleCannotBeEmpty].Value;
                return RedirectToAction(nameof(Index));
            }

            bool userExists = await userService.UserExistsByIdAsync(model.Id);

            if (!userExists)
            {
                return NotFound();
            }

            bool deleteResult = await userService.DeleteUserAsync(model.Id);

            if (!deleteResult)
            {
                TempData["ErrorMessage"] = sharedLocalizer[FailedToDeleteUser].Value;
                return RedirectToAction(nameof(Index));
            }

            TempData["SuccessMessage"] = sharedLocalizer[UserDeleted].Value;
            return RedirectToAction(nameof(Index));
        }
    }
}
