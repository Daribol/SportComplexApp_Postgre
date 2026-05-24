using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using SportComplexApp.Common;
using SportComplexApp.Data;
using SportComplexApp.Data.Models;
using SportComplexApp.Services;
using SportComplexApp.Services.Data.Contracts;
using SportComplexApp.Web.ViewModels.Trainer;

namespace SportComplexApp.Web.Controllers
{
    public class TrainerController : BaseController
    {
        private readonly ITrainerService trainerService;
        private readonly ISportService sportService;
        private readonly UserManager<Client> userManager;
        private readonly SportComplexDbContext context;
        private readonly IStringLocalizer<SharedResource> sharedLocalizer;

        public TrainerController(ITrainerService trainerService, ISportService sportService, UserManager<Client> userManager, SportComplexDbContext context, IStringLocalizer<SharedResource> sharedLocalizer)
        {
            this.trainerService = trainerService;
            this.sportService = sportService;
            this.userManager = userManager;
            this.context = context;
            this.sharedLocalizer = sharedLocalizer;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> All(int sportId)
        {
            var trainers = await trainerService.GetTrainersBySportIdAsync(sportId);

            if (trainers == null || !trainers.Any())
            {
                return NotFound();
            }

            ViewBag.SportId = sportId;
            return View(trainers);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Browse(string? q = null, int? sportId = null, string? sortBy = null, int page = 1)
        {
            int pageSize = 6;

            var allTrainers = await trainerService.GetAllPublicAsync(q, sportId, sortBy);

            int totalItems = allTrainers.Count();
            int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            if (page < 1) page = 1;
            if (page > totalPages && totalPages > 0) page = totalPages;

            var paginatedTrainers = allTrainers
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.Sports = await sportService.GetAllAsSelectListAsync();
            ViewBag.Query = q;
            ViewBag.SelectedSportId = sportId;
            ViewBag.SortBy = sortBy;

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            return View(paginatedTrainers);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Details(int id, int? month, int? year)
        {
            var trainer = await trainerService.GetTrainerDetailsAsync(id);
            if (trainer == null) return NotFound();

            int targetMonth = month ?? DateTime.Now.Month;
            int targetYear = year ?? DateTime.Now.Year;

            var allReservations = await trainerService.GetReservationsForTrainerAsync(id);
            var monthReservations = allReservations
                .Where(r => r.ReservationDate.Month == targetMonth && r.ReservationDate.Year == targetYear)
                .ToList();

            trainer.CurrentMonth = targetMonth;
            trainer.CurrentYear = targetYear;
            trainer.Reservations = monthReservations;

            return View(trainer);
        }

        [Authorize(Roles = "Trainer")]
        public async Task<IActionResult> Reservations(int? month, int? year)
        {
            var user = await userManager.GetUserAsync(User);
            var trainerId = await trainerService.GetTrainerIdByUserId(user.Id);

            if (trainerId == null)
            {
                return NotFound();
            }

            int targetMonth = month ?? DateTime.Now.Month;
            int targetYear = year ?? DateTime.Now.Year;

            var allReservations = await trainerService.GetReservationsForTrainerAsync(trainerId.Value);

            var monthReservations = allReservations
                .Where(r => r.ReservationDate.Month == targetMonth && r.ReservationDate.Year == targetYear)
                .ToList();

            ViewBag.CurrentMonth = targetMonth;
            ViewBag.CurrentYear = targetYear;

            return View(monthReservations);
        }

        [Authorize(Roles = "Trainer")]
        [HttpPost]
        public async Task<IActionResult> CancelReservation(int reservationId)
        {
            var user = await userManager.GetUserAsync(User);
            var trainerId = await trainerService.GetTrainerIdByUserId(user.Id);

            if (trainerId == null)
            {
                return NotFound();
            }

            var reservation = await context.Reservations
                .FindAsync(reservationId);

            if (reservation == null)
            {
                TempData["ErrorMessage"] = sharedLocalizer["CancelReservationUnauthorized"].Value;
                return RedirectToAction(nameof(Reservations));
            }

            context.Reservations.Remove(reservation);
            await context.SaveChangesAsync();

            TempData["SuccessMessage"] = sharedLocalizer[SuccessfulValidationMessages.Reservation.ReservationDeleted].Value;
            return RedirectToAction(nameof(Reservations));
        }

        [HttpGet]
        public async Task<IActionResult> GetCalendarPartial(int id, int month, int year)
        {
            var trainer = await trainerService.GetTrainerDetailsAsync(id);
            if (trainer == null) return NotFound();

            var allReservations = await trainerService.GetReservationsForTrainerAsync(id);

            trainer.CurrentMonth = month;
            trainer.CurrentYear = year;
            trainer.Reservations = allReservations
                .Where(r => r.ReservationDate.Month == month && r.ReservationDate.Year == year)
                .ToList();

            return PartialView("_TrainerCalendarPartial", trainer);
        }

        [HttpGet]
        [Authorize(Roles = "Trainer")]
        public async Task<IActionResult> GetReservationsPartial(int month, int year)
        {
            var user = await userManager.GetUserAsync(User);
            var trainerId = await trainerService.GetTrainerIdByUserId(user.Id);

            if (trainerId == null)
            {
                return NotFound();
            }

            var allReservations = await trainerService.GetReservationsForTrainerAsync(trainerId.Value);

            var monthReservations = allReservations
                .Where(r => r.ReservationDate.Month == month && r.ReservationDate.Year == year)
                .ToList();

            ViewBag.CurrentMonth = month;
            ViewBag.CurrentYear = year;

            return PartialView("_TrainerReservationsCalendarPartial", monthReservations);
        }
    }
}
