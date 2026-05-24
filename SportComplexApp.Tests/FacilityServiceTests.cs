using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Moq;
using NUnit.Framework;
using SportComplexApp.Common;
using SportComplexApp.Data;
using SportComplexApp.Data.Models;
using SportComplexApp.Services.Data;
using SportComplexApp.Web.ViewModels.Facility;

namespace SportComplexApp.Tests
{
    [TestFixture]
    public class FacilityServiceTests
    {
        private SportComplexDbContext _context = null!;
        private FacilityService _service = null!;
        private Mock<IStringLocalizer<SharedResource>> _mockLocalizer = null!;

        [SetUp]
        public void Setup()
        {
            var databaseName = $"FacilityDb_{Guid.NewGuid()}";
            var options = new DbContextOptionsBuilder<SportComplexDbContext>()
                .UseInMemoryDatabase(databaseName: databaseName)
                .Options;

            _context = new TestDbContext(options);
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();

            SeedData(_context);

            _mockLocalizer = new Mock<IStringLocalizer<SharedResource>>();
            _mockLocalizer.Setup(l => l[It.IsAny<string>()])
                          .Returns((string key) => new LocalizedString(key, key));

            _service = new FacilityService(_context, _mockLocalizer.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _context?.Dispose();
        }

        private void SeedData(SportComplexDbContext context)
        {
            if (context.Facilities.Any()) return;

            context.Facilities.AddRange(
                new Facility { Id = 1, Name = "Main Arena", ImageUrl = "img1.jpg", IsDeleted = false },
                new Facility { Id = 2, Name = "Training Hall", ImageUrl = "img2.jpg", IsDeleted = false },
                new Facility { Id = 3, Name = "Old Stadium", ImageUrl = "img3.jpg", IsDeleted = true },
                new Facility { Id = 4, Name = "Community Center", ImageUrl = "img4.jpg", IsDeleted = false }
            );

            context.Sports.AddRange(
                new Sport { Id = 101, Name = "Football", IsDeleted = false, FacilityId = 1 },
                new Sport { Id = 102, Name = "Basketball", IsDeleted = true, FacilityId = 4 }
            );

            context.SaveChanges();
        }

        [Test]
        public async Task GetAllFacilitiesWithSportsAsync_ReturnsOnlyActiveFacilities_AndActiveSports()
        {
            _context.Facilities.RemoveRange(_context.Facilities);
            _context.Sports.RemoveRange(_context.Sports);
            await _context.SaveChangesAsync();

            var activeArena = new Facility { Name = "Active Arena", ImageUrl = "img1.jpg", IsDeleted = false };
            var activeHall = new Facility { Name = "Active Hall", ImageUrl = "img2.jpg", IsDeleted = false };
            var deletedStadium = new Facility { Name = "Deleted Stadium", ImageUrl = "img3.jpg", IsDeleted = true };

            activeArena.Sports.Add(new Sport { Name = "Active Football", IsDeleted = false });
            activeArena.Sports.Add(new Sport { Name = "Deleted Tennis", IsDeleted = true });

            _context.Facilities.AddRange(activeArena, activeHall, deletedStadium);
            await _context.SaveChangesAsync();

            var result = await _service.GetAllFacilitiesWithSportsAsync();

            Assert.That(result.Count(), Is.EqualTo(2));

            Assert.IsFalse(result.Any(f => f.Name == "Deleted Stadium"));

            var arenaFromResult = result.FirstOrDefault(f => f.Name == "Active Arena");
            Assert.IsNotNull(arenaFromResult);

            Assert.That(arenaFromResult!.Sports.Count, Is.EqualTo(1));
            Assert.That(arenaFromResult.Sports.First().SportName, Is.EqualTo("Active Football"));

            var hallFromResult = result.FirstOrDefault(f => f.Name == "Active Hall");
            Assert.IsNotNull(hallFromResult);
            Assert.That(hallFromResult!.Sports.Count, Is.EqualTo(0));
        }

        [Test]
        public async Task AddAsync_AddsNewFacilityToDatabase()
        {
            var model = new AddFacilityViewModel { Name = "New Gym", ImageUrl = "new.jpg" };

            await _service.AddAsync(model);

            var dbFacility = await _context.Facilities.FirstOrDefaultAsync(f => f.Name == "New Gym");
            Assert.IsNotNull(dbFacility);
            Assert.That(dbFacility!.ImageUrl, Is.EqualTo("new.jpg"));
            Assert.IsFalse(dbFacility.IsDeleted);
        }

        [Test]
        public async Task ExistsAsync_ReturnsTrue_WhenFacilityExistsAndNotDeleted()
        {
            string facilityName = "Test Arena Exists";
            var facility = new Facility
            {
                Name = facilityName,
                ImageUrl = "exists.jpg",
                IsDeleted = false
            };
            _context.Facilities.Add(facility);
            await _context.SaveChangesAsync();

            var exists = await _service.ExistsAsync(facilityName);

            Assert.IsTrue(exists);
        }

        [Test]
        public async Task ExistsAsync_ReturnsFalse_WhenFacilityIsDeletedOrNotFound()
        {
            string deletedName = "Deleted Arena";
            var deletedFacility = new Facility
            {
                Name = deletedName,
                ImageUrl = "deleted.jpg",
                IsDeleted = true
            };
            _context.Facilities.Add(deletedFacility);
            await _context.SaveChangesAsync();

            Assert.IsFalse(await _service.ExistsAsync(deletedName));

            Assert.IsFalse(await _service.ExistsAsync("Ghost Hall"));
        }

        [Test]
        public async Task GetFacilityForEditAsync_ReturnsModel_WhenFacilityIsValid()
        {
            var facility = new Facility { Name = "Edit Me Arena", ImageUrl = "edit.jpg", IsDeleted = false };
            _context.Facilities.Add(facility);
            await _context.SaveChangesAsync();

            var model = await _service.GetFacilityForEditAsync(facility.Id);

            Assert.IsNotNull(model);
            Assert.That(model!.Name, Is.EqualTo("Edit Me Arena"));
            Assert.That(model.ImageUrl, Is.EqualTo("edit.jpg"));
        }

        [Test]
        public async Task GetFacilityForEditAsync_ReturnsNull_WhenFacilityIsDeletedOrNotFound()
        {
            var deletedFacility = new Facility { Name = "Deleted Edit", ImageUrl = "del.jpg", IsDeleted = true };
            _context.Facilities.Add(deletedFacility);
            await _context.SaveChangesAsync();

            Assert.IsNull(await _service.GetFacilityForEditAsync(deletedFacility.Id));
            Assert.IsNull(await _service.GetFacilityForEditAsync(9999));
        }

        [Test]
        public async Task EditAsync_UpdatesFacilityProperties_WhenFacilityIsValid()
        {
            var model = new AddFacilityViewModel { Name = "Updated Arena", ImageUrl = "updated.jpg" };

            await _service.EditAsync(1, model);

            var dbFacility = await _context.Facilities.FindAsync(1);
            Assert.That(dbFacility!.Name, Is.EqualTo("Updated Arena"));
            Assert.That(dbFacility.ImageUrl, Is.EqualTo("updated.jpg"));
        }

        [Test]
        public async Task EditAsync_DoesNothing_WhenFacilityIsDeleted()
        {
            var deletedFacility = new Facility
            {
                Name = "Old Stadium",
                ImageUrl = "old.jpg",
                IsDeleted = true
            };
            _context.Facilities.Add(deletedFacility);
            await _context.SaveChangesAsync();

            var model = new AddFacilityViewModel { Name = "Hacked Name", ImageUrl = "hacked.jpg" };

            await _service.EditAsync(deletedFacility.Id, model);

            var dbFacility = await _context.Facilities.FindAsync(deletedFacility.Id);
            Assert.IsNotNull(dbFacility);
            Assert.That(dbFacility!.Name, Is.EqualTo("Old Stadium"));
        }

        [Test]
        public async Task EditAsync_DoesNothing_WhenFacilityNotFound()
        {
            var model = new AddFacilityViewModel { Name = "Hacked Name", ImageUrl = "hacked.jpg" };

            await _service.EditAsync(999, model);

            var dbFacility = await _context.Facilities.FindAsync(999);
            Assert.IsNull(dbFacility);
        }

        [Test]
        public async Task GetFacilityForDeleteAsync_ReturnsModel_WhenFacilityIsValid()
        {
            var facility = new Facility { Name = "Target Delete Arena", ImageUrl = "target.jpg", IsDeleted = false };
            _context.Facilities.Add(facility);
            await _context.SaveChangesAsync();

            var model = await _service.GetFacilityForDeleteAsync(facility.Id);

            Assert.IsNotNull(model);
            Assert.That(model!.Id, Is.EqualTo(facility.Id));
            Assert.That(model.Name, Is.EqualTo("Target Delete Arena"));
        }

        [Test]
        public async Task GetFacilityForDeleteAsync_ReturnsNull_WhenFacilityIsDeletedOrNotFound()
        {
            var deletedFacility = new Facility { Name = "Deleted Target", ImageUrl = "del.jpg", IsDeleted = true };
            _context.Facilities.Add(deletedFacility);
            await _context.SaveChangesAsync();

            Assert.IsNull(await _service.GetFacilityForDeleteAsync(deletedFacility.Id));
            Assert.IsNull(await _service.GetFacilityForDeleteAsync(9999));
        }

        [Test]
        public void DeleteAsync_ThrowsInvalidOperationException_WhenFacilityNotFoundOrDeleted()
        {
            Assert.ThrowsAsync<InvalidOperationException>(async () => await _service.DeleteAsync(99));
            Assert.ThrowsAsync<InvalidOperationException>(async () => await _service.DeleteAsync(3));
        }

        [Test]
        public void DeleteAsync_ThrowsInvalidOperationException_WhenFacilityHasSports()
        {
            Assert.ThrowsAsync<InvalidOperationException>(async () => await _service.DeleteAsync(1));
        }

        [Test]
        public async Task DeleteAsync_SetsIsDeletedToTrue_WhenFacilityCanBeDeleted()
        {
            var emptyFacility = new Facility
            {
                Name = "Safe Delete Hall",
                ImageUrl = "safe.jpg",
                IsDeleted = false
            };
            _context.Facilities.Add(emptyFacility);
            await _context.SaveChangesAsync();

            await _service.DeleteAsync(emptyFacility.Id);

            var dbFacility = await _context.Facilities.FindAsync(emptyFacility.Id);
            Assert.IsNotNull(dbFacility);
            Assert.IsTrue(dbFacility!.IsDeleted);
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
                modelBuilder.Ignore<Microsoft.AspNetCore.Identity.IdentityUser>();

                modelBuilder.ApplyConfigurationsFromAssembly(typeof(SportComplexDbContext).Assembly);
            }
        }
    }
}