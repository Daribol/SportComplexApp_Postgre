using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using SportComplexApp.Data;
using SportComplexApp.Data.Models;
using SportComplexApp.Services.Data;
using SportComplexApp.Web.ViewModels.Trainer;

namespace SportComplexApp.Tests
{
    [TestFixture]
    public class TrainerServiceTests
    {
        private SportComplexDbContext _context = null!;
        private TrainerService _service = null!;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<SportComplexDbContext>()
                .UseInMemoryDatabase(databaseName: $"TrainerDb_{Guid.NewGuid()}")
                .Options;

            _context = new TestDbContext(options);

            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();

            SeedData(_context);

            _service = new TrainerService(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context?.Dispose();
        }

        public void SeedData(SportComplexDbContext context)
        {
            context.Reservations.RemoveRange(context.Reservations);
            context.SportTrainers.RemoveRange(context.SportTrainers);
            context.Trainers.RemoveRange(context.Trainers);
            context.Sports.RemoveRange(context.Sports);
            context.Facilities.RemoveRange(context.Facilities);
            context.Users.RemoveRange(context.Users);
            context.SaveChanges();

            var facility = new Facility { Id = 1, Name = "Main Arena", IsDeleted = false };
            context.Facilities.Add(facility);

            var football = new Sport { Id = 1, Name = "Football", IsDeleted = false, Facility = facility };
            var tennis = new Sport { Id = 2, Name = "Tennis", IsDeleted = false, Facility = facility };
            var swimming = new Sport { Id = 3, Name = "Swimming", IsDeleted = true, Facility = facility };

            var trainer1 = new Trainer
            {
                Id = 1,
                ClientId = "client1",
                Name = "John",
                LastName = "Doe",
                ImageUrl = "john.jpg",
                Bio = "Football coach",
                IsDeleted = false,
                SportTrainers = new List<SportTrainer>()
            };

            var trainer2 = new Trainer
            {
                Id = 2,
                ClientId = "client2",
                Name = "Jane",
                LastName = "Smith",
                ImageUrl = "jane.jpg",
                Bio = "Tennis expert",
                IsDeleted = false,
                SportTrainers = new List<SportTrainer>()
            };

            var trainer3 = new Trainer
            {
                Id = 3,
                ClientId = "client3",
                Name = "Mike",
                LastName = "Brown",
                ImageUrl = "mike.jpg",
                Bio = "Swimming coach",
                IsDeleted = true,
                SportTrainers = new List<SportTrainer>()
            };

            var sportTrainers = new List<SportTrainer>
    {
        new SportTrainer { Trainer = trainer1, Sport = football },
        new SportTrainer { Trainer = trainer1, Sport = swimming },
        new SportTrainer { Trainer = trainer2, Sport = tennis }
    };

            context.Sports.AddRange(football, tennis, swimming);
            context.Trainers.AddRange(trainer1, trainer2, trainer3);
            context.SportTrainers.AddRange(sportTrainers);

            var user1 = new Client { Id = "u1", FirstName = "Alice", LastName = "Johnson" };
            var user2 = new Client { Id = "u2", FirstName = "Bob", LastName = "Brown" };

            var reservations = new List<Reservation>
    {
        new Reservation
        {
            Id = 1,
            Trainer = trainer1,
            Client = user1,
            Sport = football,
            ReservationDateTime = new DateTime(2025, 1, 1, 10, 0, 0),
            Duration = 60,
            NumberOfPeople = 2
        },
        new Reservation
        {
            Id = 2,
            Trainer = trainer1,
            Client = user2,
            Sport = tennis,
            ReservationDateTime = new DateTime(2025, 1, 2, 9, 0, 0),
            Duration = 45,
            NumberOfPeople = 1
        },
        new Reservation
        {
            Id = 3,
            Trainer = trainer2,
            Client = user1,
            Sport = tennis,
            ReservationDateTime = new DateTime(2025, 1, 3, 8, 0, 0),
            Duration = 30,
            NumberOfPeople = 3
        }
    };

            context.Reservations.AddRange(reservations);
            context.Users.AddRange(user1, user2);

            context.SaveChanges();
        }

        [Test]
        public async Task GetAllAsync_ReturnsOnlyNonDeletedTrainers()
        {
            var result = (await _service.GetAllAsync()).ToList();

            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result.Any(t => t.Id == 1));
            Assert.That(result.Any(t => t.Id == 2));
            Assert.That(result.All(t => t.Id != 3));
        }

        [Test]
        public async Task GetAllAsync_MapsSports_OnlyNonDeletedSports()
        {
            var result = (await _service.GetAllAsync()).ToList();
            var trainer1 = result.First(t => t.Id == 1);

            Assert.That(trainer1.Sports, Has.Count.EqualTo(1));
            Assert.That(trainer1.Sports.First(), Is.EqualTo("Football"));
        }

        [Test]
        public async Task GetAllAsync_MapsAllFieldsCorrectly()
        {
            var result = (await _service.GetAllAsync()).ToList();
            var trainer2 = result.First(t => t.Id == 2);

            Assert.That(trainer2.Name, Is.EqualTo("Jane"));
            Assert.That(trainer2.LastName, Is.EqualTo("Smith"));
            Assert.That(trainer2.ImageUrl, Is.EqualTo("jane.jpg"));
            Assert.That(trainer2.Sports.Single(), Is.EqualTo("Tennis"));
        }

        [Test]
        public async Task GetAllAsync_ReturnsEmptyList_WhenNoActiveTrainers()
        {
            _context.Trainers.ToList().ForEach(t => t.IsDeleted = true);
            _context.SaveChanges();

            var result = await _service.GetAllAsync();

            Assert.That(result, Is.Empty);
        }

        [Test]
        public async Task GetAllForHomeAsync_ReturnsOnlyNonDeletedTrainers()
        {
            var result = (await _service.GetAllForHomeAsync()).ToList();

            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result.All(t => t.Id != 3));
        }

        [Test]
        public async Task GetAllForHomeAsync_MapsFullNameCorrectly()
        {
            var result = (await _service.GetAllForHomeAsync()).ToList();

            var trainer1 = result.First(t => t.Id == 1);
            var trainer2 = result.First(t => t.Id == 2);

            Assert.That(trainer1.FullName, Is.EqualTo("John Doe"));
            Assert.That(trainer2.FullName, Is.EqualTo("Jane Smith"));
        }

        [Test]
        public async Task GetAllForHomeAsync_MapsImageUrlCorrectly()
        {
            var result = (await _service.GetAllForHomeAsync()).ToList();

            var trainer1 = result.First(t => t.Id == 1);
            var trainer2 = result.First(t => t.Id == 2);

            Assert.That(trainer1.ImageUrl, Is.EqualTo("john.jpg"));
            Assert.That(trainer2.ImageUrl, Is.EqualTo("jane.jpg"));
        }

        [Test]
        public async Task GetAllForHomeAsync_ReturnsEmptyList_WhenNoActiveTrainers()
        {
            _context.Trainers.ToList().ForEach(t => t.IsDeleted = true);
            _context.SaveChanges();

            var result = await _service.GetAllForHomeAsync();

            Assert.That(result, Is.Empty);
        }

        [Test]
        public async Task GetAllPublicAsync_ReturnsAllActiveTrainers_OrderedByNameThenLastName()
        {
            var result = (await _service.GetAllPublicAsync(query: null, sportId: null)).ToList();

            Assert.That(result.Select(r => r.Name), Is.EquivalentTo(new[] { "John", "Jane" }));

            var ordered = result.OrderBy(r => r.Name).ThenBy(r => r.LastName).Select(r => (r.Name, r.LastName)).ToList();
            Assert.That(result.Select(r => (r.Name, r.LastName)).ToList(), Is.EqualTo(ordered));
        }

        [Test]
        public async Task GetAllPublicAsync_FiltersBySportId_ReturnsOnlyActiveTrainersLinkedToSport()
        {
            var result = (await _service.GetAllPublicAsync(null, 2)).ToList();

            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[0].Name, Is.EqualTo("Jane"));
            Assert.That(result[0].LastName, Is.EqualTo("Smith"));
            CollectionAssert.AreEquivalent(new[] { "Tennis" }, result[0].Sports);
        }

        [Test]
        public async Task GetAllPublicAsync_FiltersBySportId_IncludesDeletedSportsInProjection_CurrentImplementation()
        {
            var result = (await _service.GetAllPublicAsync(null, 3)).ToList();

            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[0].Name, Is.EqualTo("John"));
            Assert.That(result[0].Sports, Does.Contain("Swimming"));
        }

        [Test]
        public async Task GetAllPublicAsync_FiltersBySportId_ExcludesDeletedTrainers()
        {
            _context.SportTrainers.Add(new SportTrainer { TrainerId = 3, SportId = 1 });
            await _context.SaveChangesAsync();

            var result = (await _service.GetAllPublicAsync(null, 1)).ToList();

            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[0].Name, Is.EqualTo("John"));
        }

        [Test]
        public async Task GetAllPublicAsync_AppliesBothFilters_WhenBothAreProvided()
        {
            var result = (await _service.GetAllPublicAsync("John", 1)).ToList();

            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[0].Name, Is.EqualTo("John"));
        }

        [Test]
        public async Task GetAllPublicAsync_ReturnsEmpty_WhenFiltersDoNotMatchSameTrainer()
        {
            var result = (await _service.GetAllPublicAsync("Jane", 1)).ToList();

            Assert.That(result, Is.Empty);
        }

        [Test]
        public async Task GetAllPublicAsync_ReturnsEmpty_WhenNoMatchesForQuery()
        {
            var result = (await _service.GetAllPublicAsync("No Such Name", null)).ToList();
            Assert.That(result, Is.Empty);
        }

        [Test]
        public async Task GetAllPublicAsync_ReturnsEmpty_WhenSportIdHasNoLinkedActiveTrainers()
        {
            var result = (await _service.GetAllPublicAsync(null, 999)).ToList();
            Assert.That(result, Is.Empty);
        }

        [Test]
        public async Task GetTrainerDetailsAsync_ReturnsCorrectTrainer_WhenExistsAndActive()
        {
            var result = await _service.GetTrainerDetailsAsync(1);

            Assert.IsNotNull(result);
            Assert.That(result!.Id, Is.EqualTo(1));
            Assert.That(result.Name, Is.EqualTo("John"));
            Assert.That(result.LastName, Is.EqualTo("Doe"));
            Assert.That(result.ImageUrl, Is.EqualTo("john.jpg"));
            Assert.That(result.Bio, Is.EqualTo("Football coach"));
            Assert.That(result.Sports.Count, Is.EqualTo(2));
            Assert.That(result.Sports, Does.Contain("Football"));
            Assert.That(result.Sports, Does.Contain("Swimming"));
        }

        [Test]
        public async Task GetTrainerDetailsAsync_ReturnsNull_WhenTrainerIsDeleted()
        {
            var result = await _service.GetTrainerDetailsAsync(3);
            Assert.IsNull(result);
        }

        [Test]
        public async Task GetTrainerDetailsAsync_ReturnsNull_WhenTrainerDoesNotExist()
        {
            var result = await _service.GetTrainerDetailsAsync(999);
            Assert.IsNull(result);
        }

        [Test]
        public async Task GetTrainerDetailsAsync_ReturnsTrainerWithNoSports_WhenNoSportsLinked()
        {
            var trainer2Sports = _context.SportTrainers.Where(st => st.TrainerId == 2).ToList();
            _context.SportTrainers.RemoveRange(trainer2Sports);
            await _context.SaveChangesAsync();

            var result = await _service.GetTrainerDetailsAsync(2);

            Assert.IsNotNull(result);
            Assert.That(result!.Sports, Is.Empty);
        }

        [Test]
        public async Task GetTrainersBySportIdAsync_ReturnsTrainersLinkedToGivenSport_ActiveSport()
        {
            var result = (await _service.GetTrainersBySportIdAsync(1)).ToList();

            Assert.That(result.Count, Is.EqualTo(1));
            var john = result.Single();
            Assert.That(john.Id, Is.EqualTo(1));
            Assert.That(john.Name, Is.EqualTo("John"));
            Assert.That(john.LastName, Is.EqualTo("Doe"));
            Assert.That(john.Sports, Is.EquivalentTo(new[] { "Football", "Swimming" }));
            Assert.That(john.ImageUrl, Is.EqualTo("john.jpg"));
        }

        [Test]
        public async Task GetTrainersBySportIdAsync_ReturnsTrainersLinkedToGivenSport_AnotherActiveSport()
        {
            var result = (await _service.GetTrainersBySportIdAsync(2)).ToList();

            Assert.That(result.Count, Is.EqualTo(1));
            var jane = result.Single();
            Assert.That(jane.Id, Is.EqualTo(2));
            Assert.That(jane.Name, Is.EqualTo("Jane"));
            Assert.That(jane.LastName, Is.EqualTo("Smith"));
            Assert.That(jane.Sports, Is.EquivalentTo(new[] { "Tennis" }));
            Assert.That(jane.ImageUrl, Is.EqualTo("jane.jpg"));
        }

        [Test]
        public async Task GetTrainersBySportIdAsync_IncludesTrainersForDeletedSport_WhenLinked()
        {
            var result = (await _service.GetTrainersBySportIdAsync(3)).ToList();

            Assert.That(result.Count, Is.EqualTo(1));
            var john = result.Single();
            Assert.That(john.Id, Is.EqualTo(1));
            Assert.That(john.Sports, Does.Contain("Swimming"));
        }

        [Test]
        public async Task GetTrainersBySportIdAsync_ExcludesDeletedTrainers_EvenIfLinked()
        {
            _context.SportTrainers.Add(new SportTrainer { TrainerId = 3, SportId = 1 });
            await _context.SaveChangesAsync();

            var result = (await _service.GetTrainersBySportIdAsync(1)).ToList();

            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result.Single().Id, Is.EqualTo(1));
        }

        [Test]
        public async Task GetTrainersBySportIdAsync_ReturnsEmpty_WhenNoTrainersLinked()
        {
            var result = (await _service.GetTrainersBySportIdAsync(999)).ToList();
            Assert.That(result, Is.Empty);
        }

        [Test]
        public async Task GetTrainerIdByUserId_ReturnsId_WhenTrainerExistsAndActive()
        {
            var result = await _service.GetTrainerIdByUserId("client1");

            Assert.IsNotNull(result);
            Assert.That(result, Is.EqualTo(1));
        }

        [Test]
        public async Task GetTrainerIdByUserId_ReturnsNull_WhenTrainerDoesNotExist()
        {
            var result = await _service.GetTrainerIdByUserId("non-existing");
            Assert.IsNull(result);
        }

        [Test]
        public async Task GetTrainerIdByUserId_ReturnsNull_WhenTrainerIsDeleted()
        {
            var result = await _service.GetTrainerIdByUserId("client3");
            Assert.IsNull(result);
        }

        [Test]
        public async Task GetTrainerIdByUserId_ReturnsCorrectId_ForDifferentUser()
        {
            var result = await _service.GetTrainerIdByUserId("client2");

            Assert.IsNotNull(result);
            Assert.That(result, Is.EqualTo(2));
        }

        [Test]
        public async Task GetReservationsForTrainerAsync_ReturnsReservationsForGivenTrainer()
        {
            var result = (await _service.GetReservationsForTrainerAsync(1)).ToList();

            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result.All(r => r.ClientName == "Alice Johnson" || r.ClientName == "Bob Brown"));
            Assert.That(result.All(r => r.SportName == "Football" || r.SportName == "Tennis"));
        }

        [Test]
        public async Task GetReservationsForTrainerAsync_ReturnsReservationsSortedByDate()
        {
            var result = (await _service.GetReservationsForTrainerAsync(1)).ToList();
            Assert.That(result[0].ReservationDate, Is.LessThan(result[1].ReservationDate));
        }

        [Test]
        public async Task GetReservationsForTrainerAsync_MapsAllFieldsCorrectly()
        {
            var result = (await _service.GetReservationsForTrainerAsync(1)).ToList();
            var first = result.First();

            Assert.That(first.Id, Is.EqualTo(1));
            Assert.That(first.ClientName, Is.EqualTo("Alice Johnson"));
            Assert.That(first.SportName, Is.EqualTo("Football"));
            Assert.That(first.ReservationDate, Is.EqualTo(new DateTime(2025, 1, 1, 10, 0, 0)));
            Assert.That(first.Duration, Is.EqualTo(60));
            Assert.That(first.NumberOfPeople, Is.EqualTo(2));
        }

        [Test]
        public async Task GetReservationsForTrainerAsync_ReturnsEmpty_WhenNoReservations()
        {
            var result = await _service.GetReservationsForTrainerAsync(999);
            Assert.That(result, Is.Empty);
        }

        [Test]
        public async Task GetSportsAsSelectListAsync_ReturnsOnlyActiveSports()
        {
            var result = (await _service.GetSportsAsSelectListAsync()).ToList();

            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result.All(s => s.Text != "Swimming"));
        }

        [Test]
        public async Task GetSportsAsSelectListAsync_OrdersSportsByName()
        {
            var result = (await _service.GetSportsAsSelectListAsync()).ToList();

            var expectedOrder = result.OrderBy(s => s.Text).Select(s => s.Text).ToList();
            Assert.That(result.Select(s => s.Text), Is.EqualTo(expectedOrder));
        }

        [Test]
        public async Task GetSportsAsSelectListAsync_MapsValueAndTextCorrectly()
        {
            var result = (await _service.GetSportsAsSelectListAsync()).ToList();

            var football = result.First(s => s.Text == "Football");
            Assert.That(football.Value, Is.EqualTo("1"));
            Assert.That(football.Text, Is.EqualTo("Football"));
        }

        [Test]
        public async Task GetSportsAsSelectListAsync_ReturnsEmptyList_WhenAllDeleted()
        {
            foreach (var sport in _context.Sports)
                sport.IsDeleted = true;

            await _context.SaveChangesAsync();

            var result = await _service.GetSportsAsSelectListAsync();
            Assert.That(result, Is.Empty);
        }

        [Test]
        public async Task AddAsync_CreatesTrainer_WithBasicFields_AndSelectedSports()
        {
            var model = new AddTrainerViewModel
            {
                Name = "Darko",
                LastName = "Tasevski",
                Bio = "UEFA Pro",
                ImageUrl = "john.jpg",
                SelectedSportIds = new List<int> { 1, 2 }
            };

            await _service.AddAsync(model);

            var added = await _context.Trainers
                .Include(t => t.SportTrainers)
                .FirstOrDefaultAsync(t => t.Name == "Darko" && t.LastName == "Tasevski");

            Assert.IsNotNull(added);
            Assert.That(added!.Bio, Is.EqualTo("UEFA Pro"));
            Assert.That(added.ImageUrl, Is.EqualTo("john.jpg"));
            Assert.That(added.SportTrainers.Select(st => st.SportId), Is.EquivalentTo(new[] { 1, 2 }));
        }

        [Test]
        public async Task AddAsync_IncreasesTrainerCount_ByOne()
        {
            var before = await _context.Trainers.CountAsync();
            var model = new AddTrainerViewModel
            {
                Name = "Haralapbi",
                LastName = "Stanchev",
                Bio = "Tennis expert",
                ImageUrl = "jane.jpg",
                SelectedSportIds = new List<int> { 2 }
            };

            await _service.AddAsync(model);
            var after = await _context.Trainers.CountAsync();

            Assert.That(after, Is.EqualTo(before + 1));
        }

        [Test]
        public async Task AddAsync_AllowsEmptySelectedSports_CreatesTrainerWithNoLinks()
        {
            var model = new AddTrainerViewModel
            {
                Name = "Alex",
                LastName = "Brown",
                Bio = "General coach",
                ImageUrl = "alex.jpg",
                SelectedSportIds = new List<int>()
            };

            await _service.AddAsync(model);

            var added = await _context.Trainers
                .Include(t => t.SportTrainers)
                .FirstOrDefaultAsync(t => t.Name == "Alex" && t.LastName == "Brown");

            Assert.IsNotNull(added);
            Assert.That(added!.SportTrainers, Is.Empty);
        }

        [Test]
        public async Task AddAsync_CanLinkDeletedSport_CurrentImplementation()
        {
            var model = new AddTrainerViewModel
            {
                Name = "Mark",
                LastName = "Lee",
                Bio = "Swim coach",
                ImageUrl = "mark.jpg",
                SelectedSportIds = new List<int> { 3 }
            };

            await _service.AddAsync(model);

            var added = await _context.Trainers
                .Include(t => t.SportTrainers)
                .FirstOrDefaultAsync(t => t.Name == "Mark" && t.LastName == "Lee");

            Assert.IsNotNull(added);
            Assert.That(added!.SportTrainers.Select(st => st.SportId), Is.EquivalentTo(new[] { 3 }));
        }

        [Test]
        public async Task AddAsync_DeduplicatesManuallyIfCallerPassesDuplicates_NotHandledByMethod()
        {
            var model = new AddTrainerViewModel
            {
                Name = "Dup",
                LastName = "Case",
                Bio = "Multi",
                ImageUrl = "dup.jpg",
                SelectedSportIds = new List<int> { 2, 1, 3 }
            };

            await _service.AddAsync(model);

            var added = await _context.Trainers
                .Include(t => t.SportTrainers)
                .FirstAsync(t => t.Name == "Dup" && t.LastName == "Case");

            Assert.That(added.SportTrainers.Select(st => st.SportId).ToList(), Is.EquivalentTo(new[] { 2, 1, 3 }));
        }

        [Test]
        public async Task GetAddTrainerFormModelAsync_ReturnsModelWithAvailableSports()
        {
            var model = await _service.GetAddTrainerFormModelAsync();

            Assert.IsNotNull(model);
            Assert.IsNotNull(model.AvailableSports);
            Assert.That(model.AvailableSports.Count(), Is.EqualTo(2));
        }

        [Test]
        public async Task GetAddTrainerFormModelAsync_AvailableSportsOrderedByName()
        {
            var model = await _service.GetAddTrainerFormModelAsync();

            var sports = model.AvailableSports.ToList();
            var expectedOrder = sports.OrderBy(s => s.Text).Select(s => s.Text).ToList();
            Assert.That(sports.Select(s => s.Text), Is.EqualTo(expectedOrder));
        }

        [Test]
        public async Task GetAddTrainerFormModelAsync_AvailableSportsHaveCorrectValueAndText()
        {
            var model = await _service.GetAddTrainerFormModelAsync();
            var football = model.AvailableSports.First(s => s.Text == "Football");

            Assert.That(football.Value, Is.EqualTo("1"));
            Assert.That(football.Text, Is.EqualTo("Football"));
        }

        [Test]
        public async Task GetAddTrainerFormModelAsync_ReturnsEmptyList_WhenNoActiveSports()
        {
            foreach (var sport in _context.Sports)
                sport.IsDeleted = true;

            await _context.SaveChangesAsync();

            var model = await _service.GetAddTrainerFormModelAsync();

            Assert.That(model.AvailableSports, Is.Empty);
        }

        [Test]
        public async Task GetTrainerForEditAsync_ReturnsModel_WhenTrainerExistsAndActive()
        {
            var vm = await _service.GetTrainerForEditAsync(1);

            Assert.IsNotNull(vm);
            Assert.That(vm!.Name, Is.EqualTo("John"));
            Assert.That(vm.LastName, Is.EqualTo("Doe"));
            Assert.That(vm.Bio, Is.EqualTo("Football coach"));
            Assert.That(vm.ImageUrl, Is.EqualTo("john.jpg"));

            Assert.That(vm.SelectedSportIds, Is.EquivalentTo(new[] { 1, 3 }));

            Assert.That(vm.AvailableSports.Count(), Is.EqualTo(2));
            CollectionAssert.AreEqual(
                vm.AvailableSports.Select(s => s.Text).OrderBy(x => x).ToList(),
                vm.AvailableSports.Select(s => s.Text).ToList()
            );
        }

        [Test]
        public async Task GetTrainerForEditAsync_ReturnsNull_WhenTrainerIsDeleted()
        {
            var vm = await _service.GetTrainerForEditAsync(3);
            Assert.IsNull(vm);
        }

        [Test]
        public async Task GetTrainerForEditAsync_ReturnsNull_WhenTrainerDoesNotExist()
        {
            var vm = await _service.GetTrainerForEditAsync(999);
            Assert.IsNull(vm);
        }

        [Test]
        public async Task GetTrainerForEditAsync_SelectedSportIdsEmpty_WhenNoLinks()
        {
            var links = _context.SportTrainers.Where(st => st.TrainerId == 2).ToList();
            _context.SportTrainers.RemoveRange(links);
            await _context.SaveChangesAsync();

            var vm = await _service.GetTrainerForEditAsync(2);

            Assert.IsNotNull(vm);
            Assert.That(vm!.SelectedSportIds, Is.Empty);
            Assert.That(vm.AvailableSports.Count(), Is.EqualTo(2));
        }

        [Test]
        public async Task GetTrainerForEditAsync_AvailableSports_HaveCorrectValueAndText()
        {
            var vm = await _service.GetTrainerForEditAsync(2);

            var football = vm!.AvailableSports.First(s => s.Text == "Football");
            Assert.That(football.Value, Is.EqualTo("1"));
            Assert.That(football.Text, Is.EqualTo("Football"));
        }

        [Test]
        public async Task EditAsync_UpdatesBasicFields_AndReplacesSportLinks()
        {
            var model = new AddTrainerViewModel
            {
                Name = "Johnny",
                LastName = "Doev",
                Bio = "UEFA Pro Coach",
                ImageUrl = "johnny.jpg",
                SelectedSportIds = new List<int> { 2 }
            };

            await _service.EditAsync(1, model);

            var updated = await _context.Trainers
                .Include(t => t.SportTrainers)
                .ThenInclude(st => st.Sport)
                .FirstAsync(t => t.Id == 1);

            Assert.That(updated.Name, Is.EqualTo("Johnny"));
            Assert.That(updated.LastName, Is.EqualTo("Doev"));
            Assert.That(updated.Bio, Is.EqualTo("UEFA Pro Coach"));
            Assert.That(updated.ImageUrl, Is.EqualTo("johnny.jpg"));

            var sportIds = updated.SportTrainers.Select(st => st.SportId).ToList();
            Assert.That(sportIds, Is.EquivalentTo(new[] { 2 }));
        }

        [Test]
        public async Task EditAsync_AllowsEmptySelectedSports_ClearsAllLinks()
        {
            var model = new AddTrainerViewModel
            {
                Name = "John",
                LastName = "Doe",
                Bio = "Football coach",
                ImageUrl = "john.jpg",
                SelectedSportIds = new List<int>()
            };

            await _service.EditAsync(1, model);

            var linksAfter = await _context.SportTrainers
                .Where(st => st.TrainerId == 1)
                .ToListAsync();

            Assert.That(linksAfter, Is.Empty);
        }

        [Test]
        public async Task EditAsync_DoesNothing_WhenTrainerIsDeleted()
        {
            var model = new AddTrainerViewModel
            {
                Name = "ShouldNot",
                LastName = "Change",
                Bio = "Nope",
                ImageUrl = "x.jpg",
                SelectedSportIds = new List<int> { 1, 2 }
            };

            await _service.EditAsync(3, model);

            var still = await _context.Trainers
                .Include(t => t.SportTrainers)
                .FirstAsync(t => t.Id == 3);

            Assert.That(still.Name, Is.EqualTo("Mike"));
            Assert.That(still.LastName, Is.EqualTo("Brown"));
            Assert.That(still.Bio, Is.EqualTo("Swimming coach"));
            Assert.That(still.ImageUrl, Is.EqualTo("mike.jpg"));
            Assert.That(still.SportTrainers, Is.Empty);
        }

        [Test]
        public async Task EditAsync_DoesNothing_WhenTrainerDoesNotExist()
        {
            var model = new AddTrainerViewModel
            {
                Name = "Nobody",
                LastName = "Here",
                Bio = "N/A",
                ImageUrl = "n/a.jpg",
                SelectedSportIds = new List<int> { 1 }
            };
            var beforeCount = await _context.Trainers.CountAsync();
            var beforeLinks = await _context.SportTrainers.CountAsync();

            await _service.EditAsync(999, model);

            var afterCount = await _context.Trainers.CountAsync();
            var afterLinks = await _context.SportTrainers.CountAsync();

            Assert.That(afterCount, Is.EqualTo(beforeCount));
            Assert.That(afterLinks, Is.EqualTo(beforeLinks));
        }

        [Test]
        public async Task EditAsync_PersistsChangesInDatabase()
        {
            var model = new AddTrainerViewModel
            {
                Name = "Jane-Updated",
                LastName = "Smith-Updated",
                Bio = "Updated Bio",
                ImageUrl = "jane-new.jpg",
                SelectedSportIds = new List<int> { 1, 2 }
            };

            await _service.EditAsync(2, model);

            var jane = await _context.Trainers
                .Include(t => t.SportTrainers)
                .FirstAsync(t => t.Id == 2);

            Assert.That(jane.Name, Is.EqualTo("Jane-Updated"));
            Assert.That(jane.LastName, Is.EqualTo("Smith-Updated"));
            Assert.That(jane.Bio, Is.EqualTo("Updated Bio"));
            Assert.That(jane.ImageUrl, Is.EqualTo("jane-new.jpg"));
            Assert.That(jane.SportTrainers.Select(st => st.SportId), Is.EquivalentTo(new[] { 1, 2 }));
        }

        [Test]
        public async Task GetTrainerForDeleteAsync_ReturnsModel_WhenTrainerExistsAndActive()
        {
            var vm = await _service.GetTrainerForDeleteAsync(1);

            Assert.IsNotNull(vm);
            Assert.That(vm!.Id, Is.EqualTo(1));
            Assert.That(vm.Name, Is.EqualTo("John"));
            Assert.That(vm.LastName, Is.EqualTo("Doe"));
        }

        [Test]
        public async Task GetTrainerForDeleteAsync_ReturnsNull_WhenTrainerIsDeleted()
        {
            var vm = await _service.GetTrainerForDeleteAsync(3);
            Assert.IsNull(vm);
        }

        [Test]
        public async Task GetTrainerForDeleteAsync_ReturnsNull_WhenTrainerDoesNotExist()
        {
            var vm = await _service.GetTrainerForDeleteAsync(999);
            Assert.IsNull(vm);
        }

        [Test]
        public async Task GetTrainerForDeleteAsync_MapsOnlyRequiredFields()
        {
            var vm = await _service.GetTrainerForDeleteAsync(2);

            Assert.IsNotNull(vm);
            Assert.That(vm!.Id, Is.EqualTo(2));
            Assert.That(vm.Name, Is.EqualTo("Jane"));
            Assert.That(vm.LastName, Is.EqualTo("Smith"));
        }

        [Test]
        public async Task DeleteAsync_SetsIsDeletedTrue_WhenTrainerExistsAndActive()
        {
            await _service.DeleteAsync(1);

            var trainer = await _context.Trainers.FindAsync(1);
            Assert.IsNotNull(trainer);
            Assert.IsTrue(trainer!.IsDeleted);
        }

        [Test]
        public async Task DeleteAsync_DoesNothing_WhenTrainerDoesNotExist()
        {
            var beforeCount = await _context.Trainers.CountAsync();

            await _service.DeleteAsync(999);

            var afterCount = await _context.Trainers.CountAsync();
            Assert.That(afterCount, Is.EqualTo(beforeCount));
            Assert.That(await _context.Trainers.AnyAsync(t => t.Id == 999), Is.False);
        }

        [Test]
        public async Task DeleteAsync_DoesNothing_WhenTrainerIsAlreadyDeleted()
        {
            var beforeState = await _context.Trainers.AsNoTracking().FirstAsync(t => t.Id == 3);

            await _service.DeleteAsync(3);

            var afterState = await _context.Trainers.AsNoTracking().FirstAsync(t => t.Id == 3);
            Assert.That(afterState.IsDeleted, Is.True);
            Assert.That(afterState.Name, Is.EqualTo(beforeState.Name));
        }

        [Test]
        public async Task DeleteAsync_PersistsChangesInDatabase()
        {
            await _service.DeleteAsync(2);

            _context.ChangeTracker.Clear();
            var trainer = await _context.Trainers.SingleAsync(t => t.Id == 2);
            Assert.IsTrue(trainer.IsDeleted);
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