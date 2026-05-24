using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using SportComplexApp.Common;
using SportComplexApp.Services.Data.Contracts;
using SportComplexApp.Web.Controllers;
using SportComplexApp.Web.ViewModels.Spa;
using static SportComplexApp.Common.ErrorMessages.SpaService;
using static SportComplexApp.Common.SuccessfulValidationMessages.SpaService;

namespace SportComplexApp.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class SpaServicesManagementController : BaseController
    {
        private readonly ISpaService spaService;
        private readonly IStringLocalizer<SharedResource> sharedLocalizer;

        public SpaServicesManagementController(ISpaService spaService, IStringLocalizer<SharedResource> sharedLocalizer)
        {
            this.spaService = spaService;
            this.sharedLocalizer = sharedLocalizer;
        }

        [HttpGet]
        public async Task<IActionResult> All(string? searchQuery = null, string? sortBy = null, int page = 1)
        {
            int pageSize = 8;

            var model = await spaService.GetAllSpaServicesAsync(searchQuery, sortBy);

            int totalItems = model.Count();
            int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            if (page < 1) page = 1;
            if (page > totalPages && totalPages > 0) page = totalPages;

            var paginatedModel = model
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.SearchQuery = searchQuery;
            ViewBag.SortBy = sortBy;

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            return View(paginatedModel);
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            return View(new AddSpaServiceViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddSpaServiceViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (await spaService.ExistsAsync(model.Name))
            {
                TempData["ErrorMessage"] = sharedLocalizer[SpaServiceAlreadyExists].Value;
                return View(model);
            }
            await spaService.AddAsync(model);
            TempData["SuccessMessage"] = sharedLocalizer[SpaServiceCreated].Value;
            return RedirectToAction(nameof(All));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var model = await spaService.GetForEditAsync(id);
            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, AddSpaServiceViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                await spaService.EditAsync(id, model);
                TempData["SuccessMessage"] = sharedLocalizer[SpaServiceUpdated].Value;
                return RedirectToAction(nameof(All));
            }
            catch (DbUpdateConcurrencyException)
            {
                ModelState.AddModelError(string.Empty, sharedLocalizer["ConcurrencyError"]);
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var model = await spaService.GetForDeleteAsync(id);
            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await spaService.DeleteAsync(id);
            TempData["SuccessMessage"] = sharedLocalizer[SpaServiceDeleted].Value;
            return RedirectToAction(nameof(All));
        }
    }
}
