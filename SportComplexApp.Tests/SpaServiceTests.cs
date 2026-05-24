using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Moq;
using NUnit.Framework;
using SportComplexApp.Common;
using SportComplexApp.Data;
using SportComplexApp.Data.Models;
using SportComplexApp.Services.Data;
using SportComplexApp.Web.ViewModels.Spa;
using SpaService = SportComplexApp.Services.Data.SpaService;
using SpaServiceEntity = SportComplexApp.Data.Models.SpaService;

namespace SportComplexApp.Tests
{
    [TestFixture]
    public class SpaServiceTests
    {
        private SportComplexDbContext _context = null!;
        private SpaService _service = null!;
        private Mock<IStringLocalizer<SharedResource>> _mockLocalizer = null!;
        private Mock<TimeProvider> _mockTimeProvider = null!;

        private readonly DateTimeOffset _fixedTime = new DateTimeOffset(2025, 6, 1, 12, 0, 0, TimeSpan.Zero);

        [SetUp]
        public void Setup()
        {
            var databaseName = $"SpaDb_{Guid.NewGuid()}";
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

            _service = new SpaService(_context, _mockTimeProvider.Object, _mockLocalizer.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _context?.Dispose();
        }

        [Test]
        public async Task AddAsync_AddsSpaServiceToDatabase()
        {
            var model = new AddSpaServiceViewModel { Name = "Sauna", Price = 20, Duration = 60, Description = "D", ProcedureDetails = "P" };
            await _service.AddAsync(model);

            var dbService = await _context.SpaServices.FirstOrDefaultAsync(s => s.Name == "Sauna");
            Assert.IsNotNull(dbService);
            Assert.That(dbService!.Price, Is.EqualTo(20));
        }

        [Test]
        public async Task GetForEditAsync_ReturnsModel_WhenValid()
        {
            var spa = new SpaServiceEntity { Name = "Edit Me", Price = 10, Duration = 30, IsDeleted = false, Description = "D", ProcedureDetails = "P" };
            _context.SpaServices.Add(spa);
            await _context.SaveChangesAsync();

            var result = await _service.GetForEditAsync(spa.Id);
            Assert.IsNotNull(result);
            Assert.That(result!.Name, Is.EqualTo("Edit Me"));
        }

        [Test]
        public async Task GetForEditAsync_ReturnsNull_WhenDeletedOrNotFound()
        {
            var spa = new SpaServiceEntity { Name = "Del", IsDeleted = true, Description = "D", ProcedureDetails = "P" };
            _context.SpaServices.Add(spa);
            await _context.SaveChangesAsync();

            Assert.IsNull(await _service.GetForEditAsync(spa.Id));
            Assert.IsNull(await _service.GetForEditAsync(999));
        }

        [Test]
        public async Task EditAsync_UpdatesSpaService_WhenValid()
        {
            var spa = new SpaServiceEntity { Name = "Old", Price = 10, Duration = 30, IsDeleted = false, Description = "D", ProcedureDetails = "P" };
            _context.SpaServices.Add(spa);
            await _context.SaveChangesAsync();

            var editModel = new AddSpaServiceViewModel { Name = "New", Price = 50, Duration = 45, Description = "New D", ProcedureDetails = "New P" };
            await _service.EditAsync(spa.Id, editModel);

            var updated = await _context.SpaServices.FindAsync(spa.Id);
            Assert.That(updated!.Name, Is.EqualTo("New"));
        }

        [Test]
        public async Task EditAsync_DoesNothing_WhenDeletedOrNotFound()
        {
            var spa = new SpaServiceEntity { Name = "Old", IsDeleted = true, Description = "D", ProcedureDetails = "P" };
            _context.SpaServices.Add(spa);
            await _context.SaveChangesAsync();

            var editModel = new AddSpaServiceViewModel { Name = "Hacked", Description = "H", ProcedureDetails = "H" };
            await _service.EditAsync(spa.Id, editModel);
            await _service.EditAsync(999, editModel);

            var dbSpa = await _context.SpaServices.FindAsync(spa.Id);
            Assert.That(dbSpa!.Name, Is.EqualTo("Old"));
        }

        [Test]
        public async Task GetForDeleteAsync_ReturnsModel_WhenValid()
        {
            var spa = new SpaServiceEntity { Name = "Delete Target", IsDeleted = false, Description = "D", ProcedureDetails = "P" };
            _context.SpaServices.Add(spa);
            await _context.SaveChangesAsync();

            var result = await _service.GetForDeleteAsync(spa.Id);
            Assert.IsNotNull(result);
            Assert.That(result!.Name, Is.EqualTo("Delete Target"));
        }

        [Test]
        public async Task GetForDeleteAsync_ReturnsNull_WhenDeletedOrNotFound()
        {
            var spa = new SpaServiceEntity { Name = "Already Del", IsDeleted = true, Description = "D", ProcedureDetails = "P" };
            _context.SpaServices.Add(spa);
            await _context.SaveChangesAsync();

            Assert.IsNull(await _service.GetForDeleteAsync(spa.Id));
            Assert.IsNull(await _service.GetForDeleteAsync(999));
        }

        [Test]
        public async Task DeleteAsync_SoftDeletesSpaService()
        {
            var spa = new SpaServiceEntity { Name = "To Delete", IsDeleted = false, Description = "D", ProcedureDetails = "P" };
            _context.SpaServices.Add(spa);
            await _context.SaveChangesAsync();

            await _service.DeleteAsync(spa.Id);

            var deleted = await _context.SpaServices.FindAsync(spa.Id);
            Assert.IsTrue(deleted!.IsDeleted);
        }

        [Test]
        public async Task ExistsAsync_ReturnsTrue_WhenServiceIsActive()
        {
            var spa = new SpaServiceEntity { Name = "Massage", IsDeleted = false, Description = "D", ProcedureDetails = "P" };
            _context.SpaServices.Add(spa);
            await _context.SaveChangesAsync();

            Assert.IsTrue(await _service.ExistsAsync("Massage"));
            Assert.IsFalse(await _service.ExistsAsync("NonExisting"));
        }

        [Test]
        public async Task GetAllSpaServicesAsync_FiltersAndSortsCorrectly()
        {
            _context.SpaServices.RemoveRange(_context.SpaServices);
            await _context.SaveChangesAsync();

            _context.SpaServices.AddRange(
                new SpaServiceEntity { Name = "Aromatherapy", Price = 50, IsDeleted = false, Description = "D", ProcedureDetails = "P" },
                new SpaServiceEntity { Name = "Basic Massage", Price = 30, IsDeleted = false, Description = "D", ProcedureDetails = "P" },
                new SpaServiceEntity { Name = "Deleted", Price = 10, IsDeleted = true, Description = "D", ProcedureDetails = "P" }
            );
            await _context.SaveChangesAsync();

            var all = await _service.GetAllSpaServicesAsync(null, "price_desc");

            Assert.That(all.Count(), Is.EqualTo(2));
            Assert.That(all.First().Name, Is.EqualTo("Aromatherapy"));

            var searched = await _service.GetAllSpaServicesAsync("Basic");
            Assert.That(searched.Count(), Is.EqualTo(1));
        }

        [Test]
        public async Task GetAllSpaServicesPaginationAsync_ReturnsCorrectFilteredAndPagedData_AdjustsPage()
        {
            _context.SpaServices.AddRange(
                new SpaServiceEntity { Name = "Spa 1", Duration = 60, IsDeleted = false, Description = "D", ProcedureDetails = "P" },
                new SpaServiceEntity { Name = "Spa 2", Duration = 90, IsDeleted = false, Description = "D", ProcedureDetails = "P" }
            );
            await _context.SaveChangesAsync();

            var result = await _service.GetAllSpaServicesPaginationAsync(
                searchQuery: "Spa", minDuration: 50, maxDuration: 100, sortBy: "name_desc", currentPage: 5, spaPerPage: 5);

            Assert.That(result.SpaServices.Count(), Is.EqualTo(2));
            Assert.That(result.SpaServices.First().Name, Is.EqualTo("Spa 2"));
            Assert.That(result.CurrentPage, Is.EqualTo(1));
        }

        [Test]
        public async Task GetAllForHomeAsync_ReturnsOnlyActiveServices()
        {
            _context.SpaServices.RemoveRange(_context.SpaServices);
            await _context.SaveChangesAsync();

            _context.SpaServices.Add(new SpaServiceEntity { Name = "Home Spa", IsDeleted = false, Description = "D", ProcedureDetails = "P" });
            await _context.SaveChangesAsync();

            var result = await _service.GetAllForHomeAsync();

            Assert.That(result.Count(), Is.EqualTo(1));
            Assert.That(result.First().Name, Is.EqualTo("Home Spa"));
        }

        [Test]
        public async Task GetSpaDetailsByIdAsync_ReturnsDetailsOrNull()
        {
            var spa = new SpaServiceEntity { Name = "Details Spa", Description = "Desc", IsDeleted = false, ProcedureDetails = "P" };
            _context.SpaServices.Add(spa);
            await _context.SaveChangesAsync();

            var details = await _service.GetSpaDetailsByIdAsync(spa.Id);
            Assert.IsNotNull(details);
            Assert.That(details!.Description, Is.EqualTo("Desc"));

            Assert.IsNull(await _service.GetSpaDetailsByIdAsync(999));
        }

        [Test]
        public async Task GetSpaServicesCountAsync_ReturnsCorrectCount()
        {
            _context.SpaServices.Add(new SpaServiceEntity { Name = "Count Spa", Duration = 60, IsDeleted = false, Description = "D", ProcedureDetails = "P" });
            await _context.SaveChangesAsync();

            var count = await _service.GetSpaServicesCountAsync("Count", 30, 90);
            Assert.That(count, Is.EqualTo(1));
        }

        [Test]
        public async Task GetSpaServiceByIdAsync_ReturnsForm_WithTomorrowDate()
        {
            var spa = new SpaServiceEntity { Name = "Form Spa", IsDeleted = false, Description = "D", ProcedureDetails = "P" };
            _context.SpaServices.Add(spa);
            await _context.SaveChangesAsync();

            var form = await _service.GetSpaServiceByIdAsync(spa.Id);
            Assert.IsNotNull(form);
            Assert.That(form!.SpaServiceName, Is.EqualTo("Form Spa"));
            Assert.That(form.ReservationDate.Date, Is.EqualTo(_fixedTime.DateTime.AddDays(1).Date));

            Assert.IsNull(await _service.GetSpaServiceByIdAsync(999));
        }

        [Test]
        public async Task GetUserReservationsAsync_ReturnsFutureReservationsMapped()
        {
            var spa = new SpaServiceEntity { Name = "Pool", Price = 50, Duration = 60, IsDeleted = false, Description = "D", ProcedureDetails = "P" };
            _context.SpaServices.Add(spa);

            _context.SpaReservations.Add(new SpaReservation
            {
                SpaService = spa,
                ClientId = "user1",
                NumberOfPeople = 2,
                ReservationDateTime = _fixedTime.DateTime.AddDays(1)
            });
            await _context.SaveChangesAsync();

            var result = await _service.GetUserReservationsAsync("user1");
            Assert.That(result.Count(), Is.EqualTo(1));
            Assert.That(result.First().TotalPrice, Is.EqualTo(100));
        }

        [Test]
        public async Task ReservationExistsAsync_ReturnsCorrectBoolean()
        {
            var res = new SpaReservation { ClientId = "user1", SpaServiceId = 1, ReservationDateTime = _fixedTime.DateTime.AddDays(1) };
            _context.SpaReservations.Add(res);
            await _context.SaveChangesAsync();

            Assert.IsTrue(await _service.ReservationExistsAsync(res.Id, "user1"));
            Assert.IsFalse(await _service.ReservationExistsAsync(res.Id, "wrong_user"));
            Assert.IsFalse(await _service.ReservationExistsAsync(999, "user1"));
        }

        [Test]
        public async Task DeleteExpiredSpaReservationsAsync_ExecutesWithoutError()
        {
            Assert.DoesNotThrowAsync(async () => await _service.DeleteExpiredSpaReservationsAsync("user1"));
        }

        [Test]
        public async Task CreateReservationAsync_SuccessfullyCreates_WhenTimeIsValid()
        {
            var spa = new SpaServiceEntity { Name = "Pool", Duration = 60, IsDeleted = false, Description = "D", ProcedureDetails = "P" };
            _context.SpaServices.Add(spa);
            await _context.SaveChangesAsync();

            var validTime = _fixedTime.DateTime.AddDays(1).Date.AddHours(14);
            var model = new SpaReservationFormViewModel { SpaServiceId = spa.Id, ReservationDate = validTime, NumberOfPeople = 2 };

            var id = await _service.CreateReservationAsync(model, "user1");

            var reservation = await _context.SpaReservations.FindAsync(id);
            Assert.IsNotNull(reservation);
            Assert.That(reservation!.ClientId, Is.EqualTo("user1"));
        }

        [Test]
        public async Task CreateReservationAsync_Throws_WhenThereIsAnOverlapWithSpa()
        {
            var spa = new SpaServiceEntity { Name = "Pool", Duration = 60, IsDeleted = false, Description = "D", ProcedureDetails = "P" };
            _context.SpaServices.Add(spa);
            await _context.SaveChangesAsync();

            var targetTime = _fixedTime.DateTime.AddDays(1).Date.AddHours(14);

            _context.SpaReservations.Add(new SpaReservation
            {
                SpaServiceId = spa.Id,
                ClientId = "user1",
                ReservationDateTime = targetTime,
                NumberOfPeople = 1
            });
            await _context.SaveChangesAsync();

            var overlapTime = targetTime.AddMinutes(30);
            var model = new SpaReservationFormViewModel { SpaServiceId = spa.Id, ReservationDate = overlapTime };

            Assert.ThrowsAsync<InvalidOperationException>(async () => await _service.CreateReservationAsync(model, "user1"));
        }

        [Test]
        public async Task CreateReservationAsync_Throws_WhenThereIsAnOverlapWithSport()
        {
            var spa = new SpaServiceEntity { Name = "Pool", Duration = 60, IsDeleted = false, Description = "D", ProcedureDetails = "P" };
            _context.SpaServices.Add(spa);
            await _context.SaveChangesAsync();

            var targetTime = _fixedTime.DateTime.AddDays(1).Date.AddHours(14);

            var sport = new Sport { Name = "Tennis" };
            _context.Sports.Add(sport);
            _context.Reservations.Add(new Reservation
            {
                Sport = sport,
                ClientId = "user1",
                ReservationDateTime = targetTime,
                Duration = 60,
                NumberOfPeople = 1
            });
            await _context.SaveChangesAsync();

            var overlapTime = targetTime.AddMinutes(30);
            var model = new SpaReservationFormViewModel { SpaServiceId = spa.Id, ReservationDate = overlapTime };

            Assert.ThrowsAsync<InvalidOperationException>(async () => await _service.CreateReservationAsync(model, "user1"));
        }

        [Test]
        public async Task CancelReservationAsync_RemovesReservation()
        {
            var reservation = new SpaReservation { ClientId = "user1", SpaServiceId = 1, ReservationDateTime = _fixedTime.DateTime.AddDays(1) };
            _context.SpaReservations.Add(reservation);
            await _context.SaveChangesAsync();

            await _service.CancelReservationAsync(reservation.Id, "user1");

            var inDb = await _context.SpaReservations.FindAsync(reservation.Id);
            Assert.IsNull(inDb);
        }

        [Test]
        public async Task GetSpaReservationsReportAsync_ReturnsCorrectData_IncludingDeletedServices()
        {
            var client = new Client { Id = "user1", FirstName = "Ivan", LastName = "Ivanov" };
            _context.Users.Add(client);

            var deletedSpa = new SpaServiceEntity { Name = "Old Pool", Price = 50, IsDeleted = true, Description = "D", ProcedureDetails = "P" };
            _context.SpaServices.Add(deletedSpa);

            var pastReservation = new SpaReservation
            {
                Client = client,
                SpaService = deletedSpa,
                ReservationDateTime = _fixedTime.DateTime.AddDays(-5),
                NumberOfPeople = 2
            };
            _context.SpaReservations.Add(pastReservation);
            await _context.SaveChangesAsync();

            var report = await _service.GetSpaReservationsReportAsync(null, null);

            Assert.That(report.Count(), Is.EqualTo(1));
            var item = report.First();
            Assert.That(item.ClientName, Is.EqualTo("Ivan Ivanov"));
            Assert.That(item.ServiceName, Is.EqualTo("Old Pool"));
            Assert.That(item.Price, Is.EqualTo(100));
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