using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using SportComplexApp.Common;
using SportComplexApp.Services.Data.Contracts;
using static SportComplexApp.Common.ErrorMessages.Tournament;
using static SportComplexApp.Common.SuccessfulValidationMessages.Tournament;

namespace SportComplexApp.Web.Controllers
{
    public class TournamentController : BaseController
    {
        private readonly ITournamentService tournamentService;
        private readonly IStringLocalizer<SharedResource> sharedLocalizer;

        public TournamentController(ITournamentService tournamentService, IStringLocalizer<SharedResource> sharedLocalizer)
        {
            this.tournamentService = tournamentService;
            this.sharedLocalizer = sharedLocalizer;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> All(string? searchQuery = null, string? sport = null, string? sortBy = null, int page = 1)
        {
            int pageSize = 6;

            var tournaments = await tournamentService.GetAllAsync(searchQuery, sport, sortBy);

            int totalItems = tournaments.Count();
            int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            if (page < 1) page = 1;
            if (page > totalPages && totalPages > 0) page = totalPages;

            var paginatedTournaments = tournaments
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.SearchQuery = searchQuery;
            ViewBag.Sport = sport;
            ViewBag.SortBy = sortBy;

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            return View(paginatedTournaments);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Register(int id)
        {
            var userId = GetUserId();

            var tournament = await tournamentService.GetByIdAsync(id);

            if (tournament == null)
            {
                return NotFound();
            }

            var today = DateTime.Now.Date;

            if (tournament.StartDate.Date <= today && tournament.EndDate.Date >= today)
            {
                TempData["ErrorMessage"] = sharedLocalizer[ErrorMessages.Tournament.TournamentRegistrationClosed].Value;
                return RedirectToAction(nameof(All));
            }

            if (User.IsInRole("Trainer"))
            {
                TempData["ErrorMessage"] = sharedLocalizer[ErrorMessages.Tournament.TrainerCannotRegister].Value;
                return RedirectToAction(nameof(All));
            }

            var isRegistered = await tournamentService.IsUserRegisteredAsync(id, userId);

            if (isRegistered)
            {
                TempData["ErrorMessage"] = sharedLocalizer[ErrorMessages.Tournament.TournamentAlreadyRegistered].Value;
            }
            else
            {
                await tournamentService.RegisterAsync(id, userId);
                TempData["SuccessMessage"] = sharedLocalizer[SuccessfulValidationMessages.Tournament.TournamentRegistered].Value;
            }

            return RedirectToAction(nameof(MyTournaments));
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Unregister(int id)
        {
            var userId = GetUserId();

            bool success = await tournamentService.UnregisterAsync(id, userId);

            if (success)
            {
                TempData["SuccessMessage"] = sharedLocalizer[SuccessfulValidationMessages.Tournament.TournamentUnregistered].Value;
            }
            else
            {
                TempData["ErrorMessage"] = sharedLocalizer[ErrorMessages.Tournament.CannotUnregister].Value;
            }

            return RedirectToAction(nameof(MyTournaments));
        }


        [HttpGet]
        [Authorize]
        public async Task<IActionResult> MyTournaments()
        {
            var userId = GetUserId();
            var tournaments = await tournamentService.GetUserTournamentsAsync(userId);
            return View(tournaments);
        }
    }
}
