using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using SportComplexApp.Data;
using SportComplexApp.Data.Models;
using SportComplexApp.Services.Data;
using SportComplexApp.Web.ViewModels.Tournament;

namespace SportComplexApp.Tests
{
    [TestFixture]
    public class TournamentServiceTests
    {
        private SportComplexDbContext _context = null!;
        private TournamentService _service = null!;

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<SportComplexDbContext>()
                .UseInMemoryDatabase(databaseName: $"TournamentDb_{Guid.NewGuid()}")
                .Options;

            _context = new TestDbContext(options);
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();

            _service = new TournamentService(_context);
            SeedData(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context?.Dispose();
        }

        private void SeedData(SportComplexDbContext context)
        {
            context.TournamentRegistrations.RemoveRange(context.TournamentRegistrations);
            context.Tournaments.RemoveRange(context.Tournaments);
            context.Sports.RemoveRange(context.Sports);
            context.SaveChanges();

            var football = new Sport { Id = 1, Name = "Football" };
            var tennis = new Sport { Id = 2, Name = "Tennis" };

            var tournaments = new List<Tournament>
    {
        new Tournament
        {
            Id = 1,
            Name = "Champions League",
            Description = "Top clubs",
            Sport = football,
            StartDate = DateTime.Now.AddDays(30),
            EndDate = DateTime.Now.AddDays(60),
            IsDeleted = false
        },
        new Tournament
        {
            Id = 2,
            Name = "Wimbledon",
            Description = "Grand Slam Tennis",
            Sport = tennis,
            StartDate = DateTime.Now.AddDays(40),
            EndDate = DateTime.Now.AddDays(55),
            IsDeleted = false
        },
        new Tournament
        {
            Id = 3,
            Name = "Local cup",
            Description = "Amateur football",
            Sport = football,
            StartDate = DateTime.Now.AddDays(10),
            EndDate = DateTime.Now.AddDays(20),
            IsDeleted = true
        },
        new Tournament
        {
            Id = 4,
            Name = "City Open",
            Description = "tennis indoors",
            Sport = tennis,
            StartDate = DateTime.Now.AddDays(70),
            EndDate = DateTime.Now.AddDays(85),
            IsDeleted = false
        }
    };

            context.Sports.AddRange(football, tennis);
            context.Tournaments.AddRange(tournaments);
            context.SaveChanges();
        }

        [Test]
        public async Task GetAllAsync_Returns_All_NotDeleted_When_NoFilters()
        {
            var result = await _service.GetAllAsync();
            var list = result.ToList();

            Assert.That(list.Count(), Is.EqualTo(3));
            Assert.That(list.All(t => !t.IsDeleted), Is.True);
            Assert.That(list.Any(t => t.Name == "Champions League"), Is.True);
            Assert.That(list.Any(t => t.Name == "Wimbledon"), Is.True);
            Assert.That(list.Any(t => t.Name == "City Open"), Is.True);
            Assert.That(list.Any(t => t.Name == "Local cup"), Is.False);
        }

        [Test]
        public async Task GetAllAsync_Filters_By_SearchQuery_In_Name()
        {
            var result = await _service.GetAllAsync(searchQuery: "Wimbledon");
            var list = result.ToList();

            Assert.That(list.Count, Is.EqualTo(1));
            Assert.That(list[0].Name, Is.EqualTo("Wimbledon"));
        }

        [Test]
        public async Task GetAllAsync_Filters_By_SearchQuery_In_Description()
        {
            var result = await _service.GetAllAsync(searchQuery: "tennis");
            var list = result.ToList();

            Assert.That(list.Count, Is.EqualTo(1));
            Assert.That(list[0].Name, Is.EqualTo("City Open"));
        }

        [Test]
        public async Task GetAllAsync_Filters_By_Sport_CaseInsensitive_And_Trim()
        {
            var result = await _service.GetAllAsync(sport: "  tEnNiS  ");
            var list = result.ToList();

            Assert.That(list.Count, Is.EqualTo(2));
            Assert.That(list.All(x => x.Sport == "Tennis"));
        }

        [Test]
        public async Task GetAllAsync_Applies_Both_Filters_Search_And_Sport()
        {
            var result = await _service.GetAllAsync(searchQuery: "tennis", sport: "Tennis");
            var list = result.ToList();

            Assert.That(list.Count, Is.EqualTo(1));
            Assert.That(list[0].Name, Is.EqualTo("City Open"));
        }

        [Test]
        public async Task GetAllAsync_Returns_Empty_When_NoMatches()
        {
            var result = await _service.GetAllAsync(searchQuery: "no-such-text");
            var list = result.ToList();

            Assert.That(list, Is.Empty);
        }

        [Test]
        public async Task GetAllAsync_Deleted_Tournaments_Are_Excluded_Even_If_Matching()
        {
            var result = await _service.GetAllAsync(searchQuery: "football");
            var list = result.ToList();

            Assert.That(list.Any(t => t.Name == "Local cup"), Is.False);
        }

        [Test]
        public async Task GetAllAsync_Maps_Sport_Name_In_ViewModel()
        {
            var dbTournament = await _context.Tournaments.FindAsync(1);
            var result = (await _service.GetAllAsync(searchQuery: "Champions")).FirstOrDefault();

            Assert.IsNotNull(result);
            Assert.That(result!.Sport, Is.EqualTo("Football"));
            Assert.That(result.StartDate, Is.EqualTo(dbTournament!.StartDate));
            Assert.That(result.EndDate, Is.EqualTo(dbTournament.EndDate));
            Assert.That(result.Description, Is.EqualTo("Top clubs"));
        }

        [Test]
        public async Task GetAllAsync_Sport_Filter_With_Only_Whitespace_Yields_NoResults()
        {
            var result = await _service.GetAllAsync(sport: "    ");
            var list = result.ToList();

            Assert.That(list, Is.Empty);
        }

        [Test]
        public async Task GetByIdAsync_ReturnsNull_WhenIdNotFound()
        {
            var tournament = await _service.GetByIdAsync(999);
            Assert.IsNull(tournament);
        }

        [Test]
        public async Task GetByIdAsync_ReturnsNull_WhenTournamentIsDeleted()
        {
            var tournament = await _service.GetByIdAsync(3);
            Assert.IsNull(tournament);
        }

        [Test]
        public async Task GetByIdAsync_ReturnsMappedViewModel_WhenFoundAndNotDeleted()
        {
            var dbTournament = await _context.Tournaments.FindAsync(1);
            var tournament = await _service.GetByIdAsync(1);

            Assert.IsNotNull(tournament);
            Assert.That(tournament!.Id, Is.EqualTo(1));
            Assert.That(tournament.Name, Is.EqualTo("Champions League"));
            Assert.That(tournament.Sport, Is.EqualTo("Football"));
            Assert.That(tournament.StartDate, Is.EqualTo(dbTournament!.StartDate));
            Assert.That(tournament.Description, Is.EqualTo("Top clubs"));
        }

        [Test]
        public async Task RegisterAsync_Registers_WhenNotAlreadyRegistered_AddsOneRow()
        {
            int tournamentId = 1;
            string clientId = "clientId";

            await _service.RegisterAsync(tournamentId, clientId);

            var registrations = await _context.TournamentRegistrations
                .Where(r => r.TournamentId == tournamentId && r.ClientId == clientId)
                .ToListAsync();

            Assert.That(registrations.Count, Is.EqualTo(1));
            Assert.That(registrations[0].TournamentId, Is.EqualTo(tournamentId));
            Assert.That(registrations[0].ClientId, Is.EqualTo(clientId));
        }

        [Test]
        public async Task RegisterAsync_Register_IsIdempotent_CalledTwice_StillOneRow()
        {
            int tournamentId = 1;
            string userId = "test-client-id";

            await _service.RegisterAsync(tournamentId, userId);
            await _service.RegisterAsync(tournamentId, userId);

            var registrations = await _context.TournamentRegistrations
                .Where(r => r.TournamentId == tournamentId && r.ClientId == userId)
                .ToListAsync();

            Assert.That(registrations.Count, Is.EqualTo(1));
        }

        [Test]
        public async Task RegisterAsync_Register_DifferentUser_ProducesSeparateRow()
        {
            int tournamentId = 1;
            string userA = "user-a";
            string userB = "user-b";

            await _service.RegisterAsync(tournamentId, userA);
            await _service.RegisterAsync(tournamentId, userB);

            var t1All = await _context.TournamentRegistrations
                .Where(r => r.TournamentId == tournamentId)
                .ToListAsync();

            Assert.That(t1All.Count, Is.EqualTo(2));
            Assert.That(t1All.Any(r => r.ClientId == userA));
            Assert.That(t1All.Any(r => r.ClientId == userB));
        }

        [Test]
        public async Task RegisterAsync_Register_SameUser_DifferentTournament_ProducesSeparateRow()
        {
            string userId = "user-a";

            await _service.RegisterAsync(1, userId);
            await _service.RegisterAsync(2, userId);

            var userRegs = await _context.TournamentRegistrations
                .Where(r => r.ClientId == userId)
                .OrderBy(r => r.TournamentId)
                .ToListAsync();

            Assert.That(userRegs.Count, Is.EqualTo(2));
            Assert.That(userRegs[0].TournamentId, Is.EqualTo(1));
            Assert.That(userRegs[1].TournamentId, Is.EqualTo(2));
        }

        [Test]
        public async Task RegisterAsync_Register_DoesNothing_WhenPreSeededRegistrationExists()
        {
            int tournamentId = 1;
            string userId = "test-client-id";

            _context.TournamentRegistrations.Add(new TournamentRegistration
            {
                TournamentId = tournamentId,
                ClientId = userId
            });
            await _context.SaveChangesAsync();

            await _service.RegisterAsync(tournamentId, userId);

            var registrations = await _context.TournamentRegistrations
                .Where(r => r.TournamentId == tournamentId && r.ClientId == userId)
                .ToListAsync();

            Assert.That(registrations.Count, Is.EqualTo(1));
        }

        [Test]
        public async Task UnregisterAsync_Unregister_RemovesRegistration_WhenExists_AndTournamentIsInFuture()
        {
            var UserId = "test-client-id";
            _context.TournamentRegistrations.Add(new TournamentRegistration { TournamentId = 1, ClientId = UserId });
            await _context.SaveChangesAsync();

            var ok = await _service.UnregisterAsync(1, UserId);

            var remaining = await _context.TournamentRegistrations
                .Where(r => r.TournamentId == 1 && r.ClientId == UserId)
                .ToListAsync();

            Assert.IsTrue(ok);
            Assert.That(remaining, Is.Empty);
        }

        [Test]
        public async Task UnregisterAsync_Unregister_ReturnsFalse_WhenRegistrationDoesNotExist()
        {
            var UserId = "test-client-id";
            var ok = await _service.UnregisterAsync(1, UserId);

            var all = await _context.TournamentRegistrations
                .Where(r => r.TournamentId == 1 && r.ClientId == UserId)
                .ToListAsync();

            Assert.IsFalse(ok);
            Assert.That(all, Is.Empty);
        }

        [Test]
        public async Task UnregisterAsync_Unregister_ReturnsFalse_WhenTournamentAlreadyStarted()
        {
            var UserId = "test-client-id";
            var t1 = await _context.Tournaments.FindAsync(1);

            t1!.StartDate = DateTime.Now.AddDays(-1);
            await _context.SaveChangesAsync();

            _context.TournamentRegistrations.Add(new TournamentRegistration { TournamentId = 1, ClientId = UserId });
            await _context.SaveChangesAsync();

            var ok = await _service.UnregisterAsync(1, UserId);

            var stillThere = await _context.TournamentRegistrations
                .Where(r => r.TournamentId == 1 && r.ClientId == UserId)
                .ToListAsync();

            Assert.IsFalse(ok);
            Assert.That(stillThere.Count, Is.EqualTo(1));
        }

        [Test]
        public async Task UnregisterAsync_Unregister_DoesNotAffect_OtherUsersOrTournaments()
        {
            var UserId = "test-client-id";
            _context.TournamentRegistrations.Add(new TournamentRegistration { TournamentId = 1, ClientId = "other-user" });
            await _context.SaveChangesAsync();

            var ok = await _service.UnregisterAsync(1, UserId);
            var all = await _context.TournamentRegistrations.ToListAsync();

            Assert.IsFalse(ok);
            Assert.That(all.Count, Is.EqualTo(1));
            Assert.That(all[0].ClientId, Is.EqualTo("other-user"));
        }

        [Test]
        public async Task GetUserTournamentsAsync_Returns_OnlyFuture_NonDeleted_Tournaments_ForUser()
        {
            var UserId = "test-client-id";
            _context.TournamentRegistrations.AddRange(
                new TournamentRegistration { TournamentId = 1, ClientId = UserId },
                new TournamentRegistration { TournamentId = 3, ClientId = UserId },
                new TournamentRegistration { TournamentId = 4, ClientId = UserId }
            );
            await _context.SaveChangesAsync();

            var result = (await _service.GetUserTournamentsAsync(UserId)).ToList();

            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result.Any(x => x.Id == 1 && x.Name == "Champions League"));
            Assert.That(result.Any(x => x.Id == 4 && x.Name == "City Open"));
            Assert.That(result.All(x => x.Id != 3));
        }

        [Test]
        public async Task GetUserTournamentsAsync_Removes_PastRegistrations_And_ExcludesThemFromResult()
        {
            var UserId = "test-client-id";
            var t1 = await _context.Tournaments.FindAsync(1);

            t1!.StartDate = DateTime.Now.AddDays(-2);
            await _context.SaveChangesAsync();

            _context.TournamentRegistrations.AddRange(
                new TournamentRegistration { TournamentId = 1, ClientId = UserId },
                new TournamentRegistration { TournamentId = 2, ClientId = UserId }
            );
            await _context.SaveChangesAsync();

            var result = (await _service.GetUserTournamentsAsync(UserId)).ToList();

            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[0].Id, Is.EqualTo(2));

            var still1 = await _context.TournamentRegistrations.CountAsync(r => r.TournamentId == 1 && r.ClientId == UserId);
            Assert.That(still1, Is.EqualTo(0));
        }

        [Test]
        public async Task GetUserTournamentsAsync_Returns_Empty_When_UserHasNoRegistrations()
        {
            var result = (await _service.GetUserTournamentsAsync("test-client-id")).ToList();
            Assert.That(result, Is.Empty);
        }

        [Test]
        public async Task IsUserRegisteredAsync_ReturnsTrue_WhenRegistered()
        {
            var UserId = "test-client-id";
            _context.TournamentRegistrations.Add(new TournamentRegistration { TournamentId = 1, ClientId = UserId });
            await _context.SaveChangesAsync();

            var result = await _service.IsUserRegisteredAsync(1, UserId);
            Assert.IsTrue(result);
        }

        [Test]
        public async Task AddAsync_IncreasesCountByOne()
        {
            var before = await _context.Tournaments.CountAsync();

            await _service.AddAsync(new AddTournamentViewModel
            {
                Name = "New Cup",
                Description = "Desc",
                StartDate = DateTime.Now.AddDays(10),
                EndDate = DateTime.Now.AddDays(20),
                SportId = 1
            });

            var after = await _context.Tournaments.CountAsync();
            Assert.That(after, Is.EqualTo(before + 1));
        }

        [Test]
        public async Task ExistsAsync_ReturnsTrue_ForExistingActiveName()
        {
            Assert.IsTrue(await _service.ExistsAsync("Champions League"));
            Assert.IsFalse(await _service.ExistsAsync("Local cup"));
        }

        [Test]
        public async Task GetForEditAsync_ReturnsModel_WhenActive()
        {
            var vm = await _service.GetForEditAsync(1);
            Assert.IsNotNull(vm);
            Assert.That(vm!.Name, Is.EqualTo("Champions League"));
        }

        [Test]
        public async Task EditAsync_UpdatesFields_WhenActive()
        {
            var model = new AddTournamentViewModel
            {
                Name = "Updated Cup",
                Description = "Updated Desc",
                StartDate = DateTime.Now.AddDays(5),
                EndDate = DateTime.Now.AddDays(15),
                SportId = 2
            };

            await _service.EditAsync(1, model);

            var updated = await _context.Tournaments.FindAsync(1);
            Assert.That(updated!.Name, Is.EqualTo("Updated Cup"));
            Assert.That(updated.SportId, Is.EqualTo(2));
        }

        [Test]
        public async Task DeleteAsync_SoftDeletes_WhenActive()
        {
            await _service.DeleteAsync(2);

            var t2 = await _context.Tournaments.FindAsync(2);
            Assert.IsNotNull(t2);
            Assert.IsTrue(t2!.IsDeleted);
        }
        public class TestDbContext : SportComplexDbContext
        {
            public TestDbContext(DbContextOptions<SportComplexDbContext> options) : base(options) { }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Ignore<Microsoft.AspNetCore.Identity.IdentityUserLogin<string>>();
                modelBuilder.Ignore<Microsoft.AspNetCore.Identity.IdentityUserRole<string>>();
                modelBuilder.Ignore<Microsoft.AspNetCore.Identity.IdentityUserClaim<string>>();
                modelBuilder.Ignore<Microsoft.AspNetCore.Identity.IdentityUserToken<string>>();
                modelBuilder.Ignore<Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>>();
                modelBuilder.Ignore<Microsoft.AspNetCore.Identity.IdentityRole>();

                modelBuilder.ApplyConfigurationsFromAssembly(typeof(SportComplexDbContext).Assembly);
            }
        }
    }
}