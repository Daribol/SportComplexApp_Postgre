using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SportComplexApp.Data;
using SportComplexApp.Data.Models;
using SportComplexApp.Services.Data.Contracts;
using SportComplexApp.Web.ViewModels.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportComplexApp.Services.Data
{
    public class UserService : IUserService
    {
        private readonly UserManager<Client> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly SportComplexDbContext context;

        public UserService(UserManager<Client> userManager, RoleManager<IdentityRole> roleManager, SportComplexDbContext context)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.context = context;
        }
        public async Task<IEnumerable<AllUsersViewModel>> GetAllUsersAsync()
        {
            var users = await userManager.Users
                .ToListAsync();

            var userViewModels = new List<AllUsersViewModel>();

            foreach (var user in users)
            {
                var roles = await userManager.GetRolesAsync(user);

                var rolesList = roles.ToList();

                if (!rolesList.Contains("Client"))
                {
                    rolesList.Add("Client");
                }

                userViewModels.Add(new AllUsersViewModel
                {
                    Id = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Roles = rolesList
                });
            }

            return userViewModels;
        }

        public async Task<string?> GetUserEmailByIdAsync(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            return user?.Email;
        }

        public async Task<bool> UserExistsByIdAsync(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);

            return user != null;
        }
        public async Task<bool> AssignUserToRoleAsync(string userId, string role)
        {
            var user = await userManager.FindByIdAsync(userId);

            bool roleExists = await roleManager.RoleExistsAsync(role);

            if (user == null || !roleExists)
            {
                return false;
            }

            bool alreadyInRole = await userManager.IsInRoleAsync(user, role);

            if (!alreadyInRole)
            {
                var result = await userManager.AddToRoleAsync(user, role);

                if (!result.Succeeded)
                {
                    return false;
                }
            }

            if (role == "Trainer")
            {
                var trainerExists = await context.Trainers.AnyAsync(t => t.ClientId == userId && !t.IsDeleted);
                if (!trainerExists)
                {
                    var trainer = new Trainer
                    {
                        Name = user.FirstName,
                        LastName = user.LastName,
                        ClientId = user.Id,
                        IsDeleted = false
                    };
                    await context.Trainers.AddAsync(trainer);
                    await context.SaveChangesAsync();
                }
            }

            return true;
        }

        public async Task<bool> RemoveUserRoleAsync(string userId, string role)
        {
            var user = await userManager.FindByIdAsync(userId);

            bool roleExists = await roleManager.RoleExistsAsync(role);

            if (user == null || !roleExists)
            {
                return false;
            }

            bool alreadyInRole = await userManager.IsInRoleAsync(user, role);

            if (alreadyInRole)
            {
                var result = await userManager.RemoveFromRoleAsync(user, role);
                if (!result.Succeeded)
                {
                    return false;
                }
            }

            return true;
        }

        public async Task<bool> DeleteUserAsync(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return false;
            }

            var relatedRegistrations = context.Reservations
                .Where(r => r.ClientId == userId);
            context.Reservations.RemoveRange(relatedRegistrations);

            var relatedSpaReservations = context.SpaReservations
                .Where(sr => sr.ClientId == userId);
            context.SpaReservations.RemoveRange(relatedSpaReservations);

            var relatedTournamentRegistrations = context.TournamentRegistrations
                .Where(tr => tr.ClientId == userId);
            context.TournamentRegistrations.RemoveRange(relatedTournamentRegistrations);

            var trainerProfile = context.Trainers
                .FirstOrDefault(t => t.ClientId == userId);

            if (trainerProfile != null)
            {
                context.Trainers.Remove(trainerProfile);
            }

            await context.SaveChangesAsync();

            var result = await userManager.DeleteAsync(user);

            if (!result.Succeeded)
            {
                return false;
            }

            return true;
        }
    }
}
