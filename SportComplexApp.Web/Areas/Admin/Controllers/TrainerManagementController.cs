using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using SportComplexApp.Common;
using SportComplexApp.Services.Data.Contracts;
using SportComplexApp.Web.Controllers;
using SportComplexApp.Web.ViewModels.Trainer;
using static SportComplexApp.Common.ErrorMessages.Trainer;
using static SportComplexApp.Common.SuccessfulValidationMessages.Trainer;

namespace SportComplexApp.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class TrainerManagementController : BaseController
    {
        private readonly ITrainerService trainerService;
        private readonly IStringLocalizer<SharedResource> sharedLocalizer;

        public TrainerManagementController(ITrainerService trainerService, IStringLocalizer<SharedResource> sharedLocalizer)
        {
            this.trainerService = trainerService;
            this.sharedLocalizer = sharedLocalizer;
        }

        [HttpGet]
        public async Task<IActionResult> All(string? query = null, string? sortBy = null, int page = 1)
        {
            int pageSize = 8;

            var trainers = await trainerService.GetAllAsync(query, sortBy);

            int totalItems = trainers.Count();
            int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            if (page < 1) page = 1;
            if (page > totalPages && totalPages > 0) page = totalPages;

            var paginatedTrainers = trainers
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.Query = query;
            ViewBag.SortBy = sortBy;

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            return View(paginatedTrainers);
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            var model = new AddTrainerViewModel
            {
                AvailableSports = await trainerService.GetSportsAsSelectListAsync()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddTrainerViewModel model)
        {
            if (model.SelectedSportIds == null || !model.SelectedSportIds.Any())
            {
                ModelState.AddModelError(nameof(model.SelectedSportIds), sharedLocalizer[MustSelectAtLeastOneSport]);
            }

            if (!ModelState.IsValid)
            {
                model.AvailableSports = await trainerService.GetSportsAsSelectListAsync();
                return View(model);
            }

            await trainerService.AddAsync(model);
            TempData["SuccessMessage"] = sharedLocalizer[TrainerAdded].Value;
            return RedirectToAction(nameof(All));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var model = await trainerService.GetTrainerForEditAsync(id);
            if (model == null)
            {
                return NotFound();
            }

            model.AvailableSports = await trainerService.GetSportsAsSelectListAsync();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, AddTrainerViewModel model)
        {
            if (model.SelectedSportIds == null || !model.SelectedSportIds.Any())
            {
                ModelState.AddModelError(nameof(model.SelectedSportIds), sharedLocalizer[MustSelectAtLeastOneSport]);
            }

            if (!ModelState.IsValid)
            {
                model.AvailableSports = await trainerService.GetSportsAsSelectListAsync();
                return View(model);
            }

            try
            {
                await trainerService.EditAsync(id, model);
                TempData["SuccessMessage"] = sharedLocalizer[TrainerUpdated].Value;
                return RedirectToAction(nameof(All));
            }
            catch (DbUpdateConcurrencyException)
            {
                ModelState.AddModelError(string.Empty, sharedLocalizer["ConcurrencyError"]);
                model.AvailableSports = await trainerService.GetSportsAsSelectListAsync();
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var trainer = await trainerService.GetTrainerForDeleteAsync(id);
            if (trainer == null)
            {
                return NotFound();
            }

            return View(trainer);
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmDelete(int id)
        {
            await trainerService.DeleteAsync(id);
            TempData["SuccessMessage"] = sharedLocalizer[TrainerDeleted].Value;
            return RedirectToAction(nameof(All));
        }
    }
}