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

            await CreateUserWithRoleAsync(userManager, "client1@sport.com", "Client123!", "Client", "Petar", "Petrov");
            await CreateUserWithRoleAsync(userManager, "client2@sport.com", "Client123!", "Client", "Maria", "Ivanova");

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