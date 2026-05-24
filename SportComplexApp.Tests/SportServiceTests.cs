using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Moq;
using NUnit.Framework;
using SportComplexApp.Common;
using SportComplexApp.Data;
using SportComplexApp.Data.Models;
using SportComplexApp.Services;
using SportComplexApp.Web.ViewModels.Sport;

namespace SportComplexApp.Tests
{
    [TestFixture]
    public class SportServiceTests
    {
        private SportComplexDbContext _context = null!;
        private SportService _service = null!;
        private Mock<IStringLocalizer<SharedResource>> _mockLocalizer = null!;
        private Mock<TimeProvider> _mockTimeProvider = null!;

        private readonly DateTimeOffset _fixedTime = new DateTimeOffset(2025, 6, 1, 12, 0, 0, TimeSpan.Zero);

        [SetUp]
        public void Setup()
        {
            var databaseName = $"SportDb_{Guid.NewGuid()}";
            var options = new DbContextOptionsBuilder<SportComplexDbContext>()
                .UseInMemoryDatabase(databaseName: databaseName)
                .Options;

            _context = new TestDbContext(options);
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();

            _mockLocalizer = new Mock<IStringLocalizer<SharedResource>>();
            _mockLocalizer.Setup(l => l[It.IsAny<string>()])
                          .Returns((string key) => new LocalizedString(key, key));

            _mockTimeProvider = new Mock<TimeProvider>();
            _mockTimeProvider.Setup(t => t.GetUtcNow()).Returns(_fixedTime);
            _mockTimeProvider.Setup(t => t.LocalTimeZone).Returns(TimeZoneInfo.Utc);

            _service = new SportService(_context, _mockTimeProvider.Object, _mockLocalizer.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _context?.Dispose();
        }

        [Test]
        public async Task AddAsync_AddsSportToDatabase()
        {
            var facility = new Facility { Name = "Arena", IsDeleted = false };
            _context.Facilities.Add(facility);
            await _context.SaveChangesAsync();

            var model = new AddSportViewModel
            {
                Name = "Unique Test Sport",
                Price = 30m,
                Duration = 60,
                FacilityId = facility.Id,
                MinPeople = 2,
                MaxPeople = 4,
                ImageUrl = "tennis.jpg"
            };

            await _service.AddAsync(model);

            var dbSport = await _context.Sports.FirstOrDefaultAsync(s => s.Name == "Unique Test Sport");
            Assert.IsNotNull(dbSport);
            Assert.That(dbSport!.Price, Is.EqualTo(30m));
            Assert.That(dbSport.FacilityId, Is.EqualTo(facility.Id));
        }

        [Test]
        public async Task GetAddFormModelAsync_ReturnsModelWithFacilities()
        {
            _context.Facilities.RemoveRange(_context.Facilities);
            _context.Facilities.Add(new Facility { Name = "Test Hall", IsDeleted = false });
            await _context.SaveChangesAsync();

            var result = await _service.GetAddFormModelAsync();

            Assert.IsNotNull(result);
            Assert.That(result.Facilities.Count(), Is.EqualTo(1));
            Assert.That(result.Facilities.First().Text, Is.EqualTo("Test Hall"));
        }

        [Test]
        public async Task GetSportForEditAsync_ReturnsModel_WhenValid()
        {
            var facility = new Facility { Name = "Hall" };
            var sport = new Sport { Name = "Edit Sport", Price = 10, Duration = 30, Facility = facility, IsDeleted = false };
            _context.Sports.Add(sport);
            await _context.SaveChangesAsync();

            var result = await _service.GetSportForEditAsync(sport.Id);
            Assert.IsNotNull(result);
            Assert.That(result!.Name, Is.EqualTo("Edit Sport"));
        }

        [Test]
        public async Task GetSportForEditAsync_ReturnsNull_WhenDeletedOrNotFound()
        {
            var sport = new Sport { Name = "Del Sport", IsDeleted = true };
            _context.Sports.Add(sport);
            await _context.SaveChangesAsync();

            Assert.IsNull(await _service.GetSportForEditAsync(sport.Id));
            Assert.IsNull(await _service.GetSportForEditAsync(999));
        }

        [Test]
        public async Task EditAsync_UpdatesSport_WhenValid()
        {
            var facility = new Facility { Name = "Arena" };
            var sport = new Sport { Name = "Old Sport", Price = 10, Duration = 30, Facility = facility, IsDeleted = false };
            _context.Sports.Add(sport);
            await _context.SaveChangesAsync();

            var editModel = new AddSportViewModel { Name = "New Sport", Price = 50m, Duration = 45, FacilityId = facility.Id };
            await _service.EditAsync(sport.Id, editModel);

            var updated = await _context.Sports.FindAsync(sport.Id);
            Assert.That(updated!.Name, Is.EqualTo("New Sport"));
            Assert.That(updated.Price, Is.EqualTo(50m));
        }

        [Test]
        public async Task EditAsync_DoesNothing_WhenNotFound()
        {
            var editModel = new AddSportViewModel { Name = "Ghost", Price = 50m };
            Assert.DoesNotThrowAsync(async () => await _service.EditAsync(999, editModel));
        }

        [Test]
        public async Task GetSportForDeleteAsync_ReturnsModel_WhenValid()
        {
            var facility = new Facility { Name = "Hall" };
            var sport = new Sport { Name = "Delete Me", Facility = facility, IsDeleted = false };
            _context.Sports.Add(sport);
            await _context.SaveChangesAsync();

            var result = await _service.GetSportForDeleteAsync(sport.Id);
            Assert.IsNotNull(result);
            Assert.That(result!.Name, Is.EqualTo("Delete Me"));
            Assert.That(result.Facility, Is.EqualTo("Hall"));
        }

        [Test]
        public async Task GetSportForDeleteAsync_ReturnsNull_WhenDeletedOrNotFound()
        {
            var sport = new Sport { Name = "Del", IsDeleted = true };
            _context.Sports.Add(sport);
            await _context.SaveChangesAsync();

            Assert.IsNull(await _service.GetSportForDeleteAsync(sport.Id));
            Assert.IsNull(await _service.GetSportForDeleteAsync(999));
        }

        [Test]
        public async Task DeleteAsync_SoftDeletesSport()
        {
            var sport = new Sport { Name = "To Delete", IsDeleted = false };
            _context.Sports.Add(sport);
            await _context.SaveChangesAsync();

            await _service.DeleteAsync(sport.Id);

            var deleted = await _context.Sports.FindAsync(sport.Id);
            Assert.IsTrue(deleted!.IsDeleted);
        }

        [Test]
        public async Task ExistsAsync_ReturnsTrue_WhenSportIsActive()
        {
            var sport = new Sport { Name = "Squash", IsDeleted = false };
            _context.Sports.Add(sport);
            await _context.SaveChangesAsync();

            Assert.IsTrue(await _service.ExistsAsync("Squash"));
            Assert.IsFalse(await _service.ExistsAsync("NonExisting"));
        }

        [Test]
        public async Task GetAllSportsAsync_FiltersAndSortsCorrectly_AllBranches()
        {
            _context.Sports.RemoveRange(_context.Sports);
            var facility = new Facility { Name = "Complex", IsDeleted = false };

            var trainer = new Trainer { Name = "T", LastName = "Test", IsDeleted = false };

            var sport1 = new Sport { Name = "Football", Duration = 90, Facility = facility, IsDeleted = false };
            var sport2 = new Sport { Name = "Tennis", Duration = 60, Facility = facility, IsDeleted = false };
            _context.Sports.AddRange(sport1, sport2);

            _context.SportTrainers.Add(new SportTrainer { Sport = sport1, Trainer = trainer });
            await _context.SaveChangesAsync();

            var result = await _service.GetAllSportsAsync(maxDuration: 100, trainerId: trainer.Id, sortBy: "duration_asc");

            Assert.That(result.Count(), Is.EqualTo(1));
            Assert.That(result.First().Name, Is.EqualTo("Football"));

            var resultNameAsc = await _service.GetAllSportsAsync(sortBy: "name_asc");
            Assert.That(resultNameAsc.First().Name, Is.EqualTo("Football"));

            var resultDurDesc = await _service.GetAllSportsAsync(sortBy: "duration_desc");
            Assert.That(resultDurDesc.First().Name, Is.EqualTo("Football"));
        }

        [Test]
        public async Task GetAllForHomeAsync_ReturnsMappedActiveSports()
        {
            _context.Sports.RemoveRange(_context.Sports);
            _context.Sports.Add(new Sport { Name = "Home Sport", IsDeleted = false });
            await _context.SaveChangesAsync();

            var result = await _service.GetAllForHomeAsync();
            Assert.That(result.Count(), Is.EqualTo(1));
            Assert.That(result.First().Name, Is.EqualTo("Home Sport"));
        }

        [Test]
        public async Task GetAllAsSelectListAsync_ReturnsMappedSports()
        {
            _context.Sports.RemoveRange(_context.Sports);
            _context.Sports.Add(new Sport { Name = "List Sport", IsDeleted = false });
            await _context.SaveChangesAsync();

            var result = await _service.GetAllAsSelectListAsync();
            Assert.That(result.Count(), Is.EqualTo(1));
            Assert.That(result.First().Text, Is.EqualTo("List Sport"));
        }

        [Test]
        public async Task GetFacilitiesSelectListAsync_ReturnsOnlyActiveFacilities()
        {
            _context.Facilities.RemoveRange(_context.Facilities);
            _context.Facilities.AddRange(
                new Facility { Name = "Active Hall", IsDeleted = false },
                new Facility { Name = "Deleted Hall", IsDeleted = true }
            );
            await _context.SaveChangesAsync();

            var result = await _service.GetFacilitiesSelectListAsync();

            Assert.That(result.Count(), Is.EqualTo(1));
            Assert.That(result.First().Text, Is.EqualTo("Active Hall"));
        }

        [Test]
        public async Task GetReservationFormAsync_ReturnsModel_WhenValid()
        {
            var facility = new Facility { Name = "Arena" };
            var sport = new Sport { Name = "Golf", Duration = 60, Facility = facility };
            _context.Sports.Add(sport);
            await _context.SaveChangesAsync();

            var result = await _service.GetReservationFormAsync(sport.Id);

            Assert.IsNotNull(result);
            Assert.That(result!.SportName, Is.EqualTo("Golf"));
            Assert.That(result.FacilityName, Is.EqualTo("Arena"));
        }

        [Test]
        public async Task GetReservationFormAsync_ReturnsNull_WhenNotFound()
        {
            Assert.IsNull(await _service.GetReservationFormAsync(999));
        }

        [Test]
        public async Task CreateReservationAsync_SuccessfullyCreates_WhenValid()
        {
            var sport = new Sport { Name = "Football", Duration = 60, IsDeleted = false, MinPeople = 1, MaxPeople = 10 };
            _context.Sports.Add(sport);
            await _context.SaveChangesAsync();

            var validTime = _fixedTime.DateTime.AddDays(1).Date.AddHours(14);
            var model = new SportReservationFormViewModel { SportId = sport.Id, ReservationDateTime = validTime, Duration = 60, NumberOfPeople = 5 };

            var id = await _service.CreateReservationAsync(model, "user1");

            var dbReservation = await _context.Reservations.FindAsync(id);
            Assert.IsNotNull(dbReservation);
            Assert.That(dbReservation!.ClientId, Is.EqualTo("user1"));
        }

        [Test]
        public async Task CreateReservationAsync_Throws_WhenReservationIsInPast()
        {
            var sport = new Sport { Name = "Football", Duration = 60, IsDeleted = false };
            _context.Sports.Add(sport);
            await _context.SaveChangesAsync();

            var pastTime = _fixedTime.DateTime.AddDays(-1);
            var model = new SportReservationFormViewModel { SportId = sport.Id, ReservationDateTime = pastTime };

            Assert.ThrowsAsync<InvalidOperationException>(async () => await _service.CreateReservationAsync(model, "user1"));
        }

        [Test]
        public async Task CreateReservationAsync_Throws_WhenReservationTooSoon()
        {
            var sport = new Sport { Name = "Football", Duration = 60, IsDeleted = false };
            _context.Sports.Add(sport);
            await _context.SaveChangesAsync();

            var tooSoon = _fixedTime.DateTime.AddMinutes(30);
            var model = new SportReservationFormViewModel { SportId = sport.Id, ReservationDateTime = tooSoon };

            Assert.ThrowsAsync<InvalidOperationException>(async () => await _service.CreateReservationAsync(model, "user1"));
        }

        [Test]
        public async Task CreateReservationAsync_Throws_WhenReservationTooFarInFuture()
        {
            var sport = new Sport { Name = "Football", Duration = 60, IsDeleted = false };
            _context.Sports.Add(sport);
            await _context.SaveChangesAsync();

            var tooFar = _fixedTime.DateTime.AddDays(100);
            var model = new SportReservationFormViewModel { SportId = sport.Id, ReservationDateTime = tooFar };

            Assert.ThrowsAsync<InvalidOperationException>(async () => await _service.CreateReservationAsync(model, "user1"));
        }

        [Test]
        public async Task CreateReservationAsync_Throws_WhenClientHasOverlappingSportReservation()
        {
            var sport = new Sport { Name = "Football", Duration = 60, IsDeleted = false };
            _context.Sports.Add(sport);
            await _context.SaveChangesAsync();

            var targetTime = _fixedTime.DateTime.AddDays(1).Date.AddHours(14);

            _context.Reservations.Add(new Reservation
            {
                SportId = sport.Id,
                ClientId = "user1",
                ReservationDateTime = targetTime,
                Duration = 60,
                NumberOfPeople = 1
            });
            await _context.SaveChangesAsync();

            var overlapTime = targetTime.AddMinutes(30);
            var model = new SportReservationFormViewModel { SportId = sport.Id, ReservationDateTime = overlapTime, Duration = 60 };

            Assert.ThrowsAsync<InvalidOperationException>(async () => await _service.CreateReservationAsync(model, "user1"));
        }

        [Test]
        public async Task CreateReservationAsync_Throws_WhenTrainerIsBusy()
        {
            var sport = new Sport { Name = "Football", Duration = 60, IsDeleted = false };

            var trainer = new Trainer { Name = "Coach", LastName = "Coachov", IsDeleted = false };

            _context.Sports.Add(sport);
            _context.Trainers.Add(trainer);
            await _context.SaveChangesAsync();

            var targetTime = _fixedTime.DateTime.AddDays(1).Date.AddHours(14);

            _context.Reservations.Add(new Reservation
            {
                SportId = sport.Id,
                ClientId = "otherUser",
                TrainerId = trainer.Id,
                ReservationDateTime = targetTime,
                Duration = 60
            });
            await _context.SaveChangesAsync();

            var overlapTime = targetTime.AddMinutes(10);
            var model = new SportReservationFormViewModel { SportId = sport.Id, TrainerId = trainer.Id, ReservationDateTime = overlapTime, Duration = 60 };

            Assert.ThrowsAsync<InvalidOperationException>(async () => await _service.CreateReservationAsync(model, "user1"));
        }

        [Test]
        public async Task CreateReservationAsync_Throws_WhenCurrentUserIsTrainerAndIsBusy()
        {
            var sport = new Sport { Name = "Football", Duration = 60, IsDeleted = false };

            var myTrainerProfile = new Trainer { Name = "Me", LastName = "Trainerov", ClientId = "user1", IsDeleted = false };

            _context.Sports.Add(sport);
            _context.Trainers.Add(myTrainerProfile);
            await _context.SaveChangesAsync();

            var targetTime = _fixedTime.DateTime.AddDays(1).Date.AddHours(14);

            _context.Reservations.Add(new Reservation
            {
                SportId = sport.Id,
                ClientId = "otherUser",
                TrainerId = myTrainerProfile.Id,
                ReservationDateTime = targetTime,
                Duration = 60
            });
            await _context.SaveChangesAsync();

            var overlapTime = targetTime.AddMinutes(10);
            var model = new SportReservationFormViewModel { SportId = sport.Id, ReservationDateTime = overlapTime, Duration = 60 };

            Assert.ThrowsAsync<InvalidOperationException>(async () => await _service.CreateReservationAsync(model, "user1"));
        }

        [Test]
        public async Task CreateReservationAsync_Throws_WhenSpaOverlapExists()
        {
            var sport = new Sport { Name = "Football", Duration = 60, IsDeleted = false };

            var spaService = new SportComplexApp.Data.Models.SpaService
            {
                Name = "Massage",
                Duration = 60,
                IsDeleted = false,
                Description = "Relaxing massage",
                ProcedureDetails = "Full body massage"
            };

            _context.Sports.Add(sport);
            _context.SpaServices.Add(spaService);
            await _context.SaveChangesAsync();

            var targetTime = _fixedTime.DateTime.AddDays(1).Date.AddHours(14);

            _context.SpaReservations.Add(new SpaReservation
            {
                SpaServiceId = spaService.Id,
                ClientId = "user1",
                ReservationDateTime = targetTime
            });
            await _context.SaveChangesAsync();

            var overlapTime = targetTime.AddMinutes(10);
            var model = new SportReservationFormViewModel { SportId = sport.Id, ReservationDateTime = overlapTime, Duration = 60 };

            Assert.ThrowsAsync<InvalidOperationException>(async () => await _service.CreateReservationAsync(model, "user1"));
        }

        [Test]
        public async Task GetUserReservationsAsync_ReturnsMappedFutureReservations()
        {
            var facility = new Facility { Name = "Arena" };
            var sport = new Sport { Name = "Football", Price = 50m, Duration = 60, Facility = facility };
            _context.Sports.Add(sport);

            _context.Reservations.Add(new Reservation
            {
                Sport = sport,
                ClientId = "user1",
                ReservationDateTime = DateTime.Now.AddDays(1),
                Duration = 60,
                NumberOfPeople = 2
            });
            await _context.SaveChangesAsync();

            var result = await _service.GetUserReservationsAsync("user1");

            Assert.That(result.Count(), Is.EqualTo(1));
            var vm = result.First();
            Assert.That(vm.SportName, Is.EqualTo("Football"));
            Assert.That(vm.FacilityName, Is.EqualTo("Arena"));
            Assert.That(vm.TotalPrice, Is.EqualTo(100m));
        }

        [Test]
        public async Task ReservationExistsAsync_ReturnsTrueOrFalse()
        {
            var res = new Reservation { ClientId = "user1", SportId = 1, ReservationDateTime = _fixedTime.DateTime.AddDays(1) };
            _context.Reservations.Add(res);
            await _context.SaveChangesAsync();

            Assert.IsTrue(await _service.ReservationExistsAsync(res.Id, "user1"));
            Assert.IsFalse(await _service.ReservationExistsAsync(999, "user1"));
        }

        [Test]
        public async Task CancelReservationAsync_RemovesReservation()
        {
            var reservation = new Reservation { ClientId = "user1", SportId = 1, ReservationDateTime = _fixedTime.DateTime.AddDays(1) };
            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync();

            await _service.CancelReservationAsync(reservation.Id, "user1");

            var inDb = await _context.Reservations.FindAsync(reservation.Id);
            Assert.IsNull(inDb);
        }

        [Test]
        public async Task DeleteExpiredReservationsAsync_DoesNotThrow()
        {
            Assert.DoesNotThrowAsync(async () => await _service.DeleteExpiredReservationsAsync("user1"));
        }


        [Test]
        public async Task GetSportReservationsReportAsync_ReturnsData_IncludingDeletedEntities()
        {
            var client = new Client { Id = "user1", FirstName = "Ivan", LastName = "Ivanov" };
            _context.Users.Add(client);

            var deletedSport = new Sport { Name = "Old Sport", Price = 20m, IsDeleted = true };
            _context.Sports.Add(deletedSport);

            var pastReservation = new Reservation
            {
                Client = client,
                Sport = deletedSport,
                ReservationDateTime = _fixedTime.DateTime.AddDays(-5),
                NumberOfPeople = 3,
                Duration = 60
            };
            _context.Reservations.Add(pastReservation);
            await _context.SaveChangesAsync();

            var report = await _service.GetSportReservationsReportAsync(null, null);

            Assert.That(report.Count(), Is.EqualTo(1));
            var item = report.First();
            Assert.That(item.ClientName, Is.EqualTo("Ivan Ivanov"));
            Assert.That(item.SportName, Is.EqualTo("Old Sport"));
            Assert.That(item.Price, Is.EqualTo(60m));
            Assert.IsFalse(item.IsSportDeleted);
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