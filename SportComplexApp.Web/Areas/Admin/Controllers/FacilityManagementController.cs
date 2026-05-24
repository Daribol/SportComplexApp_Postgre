using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using SportComplexApp.Common;
using SportComplexApp.Services.Data.Contracts;
using SportComplexApp.Web.Controllers;
using SportComplexApp.Web.ViewModels.Facility;
using static SportComplexApp.Common.ErrorMessages.Facility;
using static SportComplexApp.Common.SuccessfulValidationMessages.Facility;

namespace SportComplexApp.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class FacilityManagementController : BaseController
    {
        private readonly IFacilityService facilityService;
        private readonly IStringLocalizer<SharedResource> sharedLocalizer;

        public FacilityManagementController(IFacilityService facilityService, IStringLocalizer<SharedResource> sharedLocalizer)
        {
            this.facilityService = facilityService;
            this.sharedLocalizer = sharedLocalizer;
        }

        [HttpGet]
        public async Task<IActionResult> All(int page = 1)
        {
            int pageSize = 6;

            var facilities = await facilityService.GetAllFacilitiesWithSportsAsync();

            int totalItems = facilities.Count();
            int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            if (page < 1) page = 1;
            if (page > totalPages && totalPages > 0) page = totalPages;

            var paginatedFacilities = facilities
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            return View(paginatedFacilities);
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View(new AddFacilityViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddFacilityViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (await facilityService.ExistsAsync(model.Name))
            {
                TempData["ErrorMessage"] = sharedLocalizer[FacilityAlreadyExists].Value;
                return View(model);
            }

            await facilityService.AddAsync(model);
            TempData["SuccessMessage"] = sharedLocalizer[FacilityAdded].Value;
            return RedirectToAction(nameof(All));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var model = await facilityService.GetFacilityForEditAsync(id);
            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AddFacilityViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                await facilityService.EditAsync(id, model);
                TempData["SuccessMessage"] = sharedLocalizer[FacilityUpdated].Value;
                return RedirectToAction(nameof(All));
            }
            catch (DbUpdateConcurrencyException)
            {
                ModelState.AddModelError(string.Empty, sharedLocalizer["ConcurrencyError"]);
                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, sharedLocalizer["UnexpectedError"] + ex.Message);
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var model = await facilityService.GetFacilityForDeleteAsync(id);
            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await facilityService.DeleteAsync(id);
                TempData["SuccessMessage"] = sharedLocalizer[FacilityDeleted].Value;
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction(nameof(All));
        }
    }
}
