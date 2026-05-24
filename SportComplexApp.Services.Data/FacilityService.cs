using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using SportComplexApp.Common;
using SportComplexApp.Data;
using SportComplexApp.Data.Models;
using SportComplexApp.Services.Data.Contracts;
using SportComplexApp.Web.ViewModels.Facility;
using static SportComplexApp.Common.ErrorMessages.Facility;

namespace SportComplexApp.Services.Data
{
    public class FacilityService : IFacilityService
    {
        private readonly SportComplexDbContext context;
        private readonly IStringLocalizer<SharedResource> _sharedLocalizer;

        public FacilityService(SportComplexDbContext context, IStringLocalizer<SharedResource> sharedLocalizer)
        {
            this.context = context;
            _sharedLocalizer = sharedLocalizer;
        }

        public async Task<IEnumerable<FacilityMasterViewModel>> GetAllFacilitiesWithSportsAsync()
        {
            var facilities = await context.Facilities
                .Where(f => !f.IsDeleted)
                .Select(f => new FacilityMasterViewModel
                {
                    Id = f.Id,
                    Name = f.Name,
                    ImageUrl = f.ImageUrl,

                    Sports = f.Sports
                        .Where(s => !s.IsDeleted)
                        .Select(s => new FacilitySportDetailViewModel
                        {
                            Id = s.Id,
                            SportName = s.Name,
                        })
                        .ToList()
                })
                .ToListAsync();

            return facilities;
        }

        public async Task AddAsync(AddFacilityViewModel model)
        {
            var facility = new Facility
            {
                Name = model.Name,
                ImageUrl = model.ImageUrl,
                IsDeleted = false
            };

            await context.Facilities.AddAsync(facility);
            await context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(string name)
        {
            return await context.Facilities
                .AnyAsync(f => f.Name == name && !f.IsDeleted);
        }

        public async Task<AddFacilityViewModel?> GetFacilityForEditAsync(int id)
        {
            var facility = await context.Facilities
                .Where(f => f.Id == id && !f.IsDeleted)
                .FirstOrDefaultAsync();

            if (facility == null) return null;

            return new AddFacilityViewModel
            {
                Id = facility.Id,
                Name = facility.Name,
                ImageUrl = facility.ImageUrl,
            };
        }

        public async Task EditAsync(int id, AddFacilityViewModel model)
        {
            var facility = await context.Facilities
                .Where(f => f.Id == id && !f.IsDeleted)
                .FirstOrDefaultAsync();

            if (facility != null)
            {
                facility.Name = model.Name;
                facility.ImageUrl = model.ImageUrl;
                await context.SaveChangesAsync();
            }
        }

        public async Task<DeleteFacilityViewModel?> GetFacilityForDeleteAsync(int id)
        {
            var facility = await context.Facilities
                .Where(f => f.Id == id && !f.IsDeleted)
                .FirstOrDefaultAsync();

            if (facility == null) return null;

            return new DeleteFacilityViewModel
            {
                Id = facility.Id,
                Name = facility.Name
            };
        }

        public async Task DeleteAsync(int id)
        {
            var facility = await context.Facilities
                .Include(f => f.Sports)
                .FirstOrDefaultAsync(f => f.Id == id && !f.IsDeleted);

            if (facility == null)
            {
                throw new InvalidOperationException(_sharedLocalizer[ErrorMessages.Facility.FacilityNotFound]);
            }

            if (facility.Sports.Any())
            {
                throw new InvalidOperationException(_sharedLocalizer[ErrorMessages.Facility.FacilityHasSports]);
            }

            facility.IsDeleted = true;
            await context.SaveChangesAsync();
        }
    }
}
