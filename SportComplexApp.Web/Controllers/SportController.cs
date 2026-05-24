using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using SportComplexApp.Common;
using SportComplexApp.Services.Data.Contracts;
using SportComplexApp.Web.ViewModels.Sport;


namespace SportComplexApp.Web.Controllers
{
    public class SportController : BaseController
    {
        private readonly IFacilityService facilityService;
        private readonly ISportService sportService;
        private readonly IStringLocalizer<SharedResource> sharedLocalizer;

        public SportController(ISportService sportService, IFacilityService facilityService, IStringLocalizer<SharedResource> sharedLocalizer)
        {
            this.sportService = sportService;
            this.facilityService = facilityService;
            this.sharedLocalizer = sharedLocalizer;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> All(string? searchQuery = null, int? minDuration = null, int? maxDuration = null, string? sortBy = null, int? trainerId = null, int page = 1)
        {
            int pageSize = 6;

            var sports = await this.sportService
                .GetAllSportsAsync(searchQuery, minDuration, maxDuration, sortBy, trainerId);

            int totalItems = sports.Count();
            int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            if (page < 1) page = 1;
            if (page > totalPages && totalPages > 0) page = totalPages;

            var paginatedSports = sports
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.SearchQuery = searchQuery;
            ViewBag.MinDuration = minDuration;
            ViewBag.MaxDuration = maxDuration;
            ViewBag.SortBy = sortBy;
            ViewBag.TrainerId = trainerId;

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            return View(paginatedSports);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Reserve(int id)
        {
            var userId = GetUserId();
            var model = await sportService.GetReservationFormAsync(id, userId);
            if (model == null)
            {
                return NotFound();
            }

            if (facilityService != null)
            {
                var allFacilities = await facilityService.GetAllFacilitiesWithSportsAsync();

                if (allFacilities != null)
                {
                    var facility = allFacilities.FirstOrDefault(f => f.Sports.Any(s => s.Id == id));
                    if (facility != null)
                    {
                        model.FacilityImageUrl = facility.ImageUrl;
                    }
                }
            }

            return View(model);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Reserve(SportReservationFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var fallback = await sportService.GetReservationFormAsync(model.SportId, GetUserId());
                if (fallback != null)
                {
                    model.SportName = fallback.SportName;
                    model.FacilityName = fallback.FacilityName;
                    model.FacilityImageUrl = fallback.FacilityImageUrl;
                    model.Trainers = fallback.Trainers;
                    model.MinDuration = fallback.MinDuration;
                    model.MaxDuration = fallback.MaxDuration;
                    model.MinPeople = fallback.MinPeople;
                    model.MaxPeople = fallback.MaxPeople;
                }
                return View(model);
            }

            var userId = GetUserId();

            try
            {
                await sportService.CreateReservationAsync(model, userId);
                TempData["SuccessMessage"] = sharedLocalizer[SuccessfulValidationMessages.Reservation.ReservationCreated].Value;
                return RedirectToAction(nameof(MyReservations));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);

                var fallback = await sportService.GetReservationFormAsync(model.SportId, GetUserId());
                if (fallback != null)
                {
                    model.SportName = fallback.SportName;
                    model.FacilityName = fallback.FacilityName;
                    model.Trainers = fallback.Trainers;
                    model.MinDuration = fallback.MinDuration;
                    model.MaxDuration = fallback.MaxDuration;
                    model.MinPeople = fallback.MinPeople;
                    model.MaxPeople = fallback.MaxPeople;
                }

                return View(model);
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> MyReservations()
        {
            var userId = GetUserId();
            await sportService.DeleteExpiredReservationsAsync(userId);

            var reservations = await sportService.GetUserReservationsAsync(userId);

            return View(reservations);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Cancel(int id)
        {
            var userId = GetUserId();
            if (!await sportService.ReservationExistsAsync(id, userId))
            {
                return NotFound();
            }

            await sportService.CancelReservationAsync(id, userId);

            TempData["SuccessMessage"] = sharedLocalizer[SuccessfulValidationMessages.Reservation.ReservationDeleted].Value;
            return RedirectToAction(nameof(MyReservations));
        }
    }
}
