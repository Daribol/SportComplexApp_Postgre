using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportComplexApp.Services.Data.Contracts;
using System.Security.Claims;
using SportComplexApp.Web.ViewModels.Tournament;

namespace SportComplexApp.WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TournamentController : ControllerBase
    {
        private readonly ITournamentService _tournament;

        public TournamentController(ITournamentService tournament)
        {
            _tournament = tournament;
        }

        protected string? GetUserId()
        {
            string? userId = null;
            userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);

            return userId;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllTournaments()
        {
            var tournaments = await _tournament.GetAllAsync();
            return Ok(tournaments);
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetTournamentsById(int id)
        {
            try
            {
                var tournaments = await _tournament.GetByIdAsync(id);
                return Ok(tournaments);
            }
            catch (ArgumentException e)
            {
                return NotFound();
            }
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CreateTournament([FromBody] AddTournamentViewModel model)
        {
            if (ModelState.IsValid == false)
            {
                return BadRequest(model);
            }

            var userId = GetUserId();
            if (userId == null)
            {
                return NotFound("User not found.");
            }

            await _tournament.AddAsync(model);

            return StatusCode(StatusCodes.Status201Created);
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> EditTournament([FromBody] AddTournamentViewModel model)
        {
            if (ModelState.IsValid == false)
            {
                return BadRequest(model);
            }

            var userId = GetUserId();
            if (userId == null)
            {
                return NotFound("User not found.");
            }

            var existingTournament = await _tournament.GetForEditAsync(model.Id);
            if (existingTournament == null)
            {
                return NotFound("Tournament not found.");
            }

            await _tournament.EditAsync(model.Id, model);
            return Ok(model);
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteTournament(int id)
        {
            var userId = GetUserId();
            if (userId == null)
            {
                return NotFound("User not found.");
            }

            var existingTournament = await _tournament.GetForDeleteAsync(id);
            if (existingTournament == null)
            {
                return NotFound("Tournament not found.");
            }

            await _tournament.DeleteAsync(id);
            return Ok();
        }
    }
}
