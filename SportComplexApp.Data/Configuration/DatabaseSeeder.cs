using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SportComplexApp.Data.Models;

namespace SportComplexApp.Data.Configuration
{
    public class DatabaseSeeder
    {
        public static async Task SeedAllAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<Client>>();
            var context = serviceProvider.GetRequiredService<SportComplexDbContext>();
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();

            string[] roles = { "Admin", "Trainer", "Client" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole { Name = role });
                }
            }

            string adminEmail = configuration["AdminSettings:Username"] ?? "admin@sport.com";
            string adminPassword = configuration["AdminSettings:Password"] ?? "Admin123!";
            await CreateUserWithRoleAsync(userManager, adminEmail, adminPassword, "Admin", "Admin", "User");

            var client1 = await CreateUserWithRoleAsync(userManager, "client1@sport.com", "Client123!", "Client", "Petar", "Petrov");
            var client2 = await CreateUserWithRoleAsync(userManager, "client2@sport.com", "Client123!", "Client", "Maria", "Ivanova");

            var unlinkedTrainers = await context.Trainers.Where(t => t.ClientId == null).ToListAsync();
            foreach (var trainer in unlinkedTrainers)
            {
                string email = $"{trainer.Name.ToLower()}.{trainer.LastName.ToLower()}@sport.com";
                var trainerUser = await CreateUserWithRoleAsync(userManager, email, "Trainer123!", "Trainer", trainer.Name, trainer.LastName);

                if (trainerUser != null)
                {
                    trainer.ClientId = trainerUser.Id;
                }
            }
            await context.SaveChangesAsync();

            if (!await context.Reservations.AnyAsync() && client1 != null && client2 != null)
            {
                var sports = await context.Sports.ToListAsync();
                var sportTrainers = await context.SportTrainers.ToListAsync();
                var random = new Random();
                var clients = new[] { client1.Id, client2.Id };

                for (int i = 0; i < 20; i++)
                {
                    var sport = sports[random.Next(sports.Count)];

                    var possibleTrainerId = sportTrainers.FirstOrDefault(st => st.SportId == sport.Id)?.TrainerId;

                    context.Reservations.Add(new Reservation
                    {
                        ClientId = clients[random.Next(clients.Length)],
                        SportId = sport.Id,
                        TrainerId = possibleTrainerId ?? 1,
                        ReservationDateTime = DateTime.UtcNow.AddDays(-random.Next(1, 30)).AddHours(random.Next(8, 20)),
                        Duration = sport.Duration,
                        NumberOfPeople = random.Next(sport.MinPeople, sport.MaxPeople + 1)
                    });
                }
                await context.SaveChangesAsync();
            }

            if (!await context.SpaReservations.AnyAsync() && client1 != null)
            {
                var spas = await context.SpaServices.ToListAsync();
                var random = new Random();

                for (int i = 0; i < 10; i++)
                {
                    context.SpaReservations.Add(new SpaReservation
                    {
                        ClientId = client1.Id,
                        SpaServiceId = spas[random.Next(spas.Count)].Id,
                        ReservationDateTime = DateTime.UtcNow.AddDays(-random.Next(1, 30)).AddHours(random.Next(10, 18))
                    });
                }
                await context.SaveChangesAsync();
            }

            if (!await context.TournamentRegistrations.AnyAsync() && client1 != null && client2 != null)
            {
                var tournaments = await context.Tournaments.ToListAsync();
                foreach (var tournament in tournaments)
                {
                    context.TournamentRegistrations.Add(new TournamentRegistration { ClientId = client1.Id, TournamentId = tournament.Id });
                    context.TournamentRegistrations.Add(new TournamentRegistration { ClientId = client2.Id, TournamentId = tournament.Id });
                }
                await context.SaveChangesAsync();
            }
        }

        private static async Task<Client?> CreateUserWithRoleAsync(UserManager<Client> userManager, string email, string password, string role, string firstName, string lastName)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                user = new Client
                {
                    UserName = email,
                    Email = email,
                    FirstName = firstName,
                    LastName = lastName,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, role);
                    return user;
                }
                return null;
            }
            return user;
        }
    }
}