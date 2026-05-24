using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using SportComplexApp.Common;
using SportComplexApp.Data;
using SportComplexApp.Data.Models;
using SportComplexApp.Services.Data.Contracts;
using SportComplexApp.Web.ViewModels.Home;
using SportComplexApp.Web.ViewModels.Report;
using SportComplexApp.Web.ViewModels.Sport;
using static SportComplexApp.Common.ErrorMessages.Reservation;
using static SportComplexApp.Common.ErrorMessages.Sport;

namespace SportComplexApp.Services
{
    public class SportService : ISportService
    {
        private readonly SportComplexDbContext context;
        private readonly TimeProvider time;
        private readonly IStringLocalizer<SharedResource> sharedLocalizer;

        public SportService(SportComplexDbContext db, TimeProvider timeProvider, IStringLocalizer<SharedResource> sharedLocalizer)
        {
            this.context = db;
            this.time = timeProvider ?? TimeProvider.System;
            this.sharedLocalizer = sharedLocalizer;
        }

        public async Task<IEnumerable<AllSportsViewModel>> GetAllSportsAsync(string? searchQuery = null, int? minDuration = null, int? maxDuration = null, string? sortBy = null, int? trainerId = null)
        {
            var query = context.Sports
                .Where(s => !s.IsDeleted && !s.Facility.IsDeleted)
                .Include(s => s.Facility)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchQuery))
            {
                query = query.Where(s => s.Name.Contains(searchQuery));
            }

            if (minDuration.HasValue)
            {
                query = query.Where(s => s.Duration >= minDuration.Value);
            }

            if (maxDuration.HasValue)
            {
                query = query.Where(s => s.Duration <= maxDuration.Value);
            }

            if (trainerId.HasValue && trainerId.Value > 0)
            {
                query = query.Where(s => s.SportTrainers.Any(st => st.TrainerId == trainerId.Value));
            }

            query = sortBy switch
            {
                "name_desc" => query.OrderByDescending(s => s.Name),
                "name_asc" => query.OrderBy(s => s.Name),
                "duration_desc" => query.OrderByDescending(s => s.Duration),
                "duration_asc" => query.OrderBy(s => s.Duration),
                _ => query.OrderBy(s => s.Id)
            };

            return await query
                .Select(s => new AllSportsViewModel
                {
                    Id = s.Id,
                    Name = s.Name,
                    Duration = s.Duration,
                    Price = s.Price,
                    Facility = s.Facility.Name,
                    ImageUrl = s.ImageUrl
                })
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<SportHomeViewModel>> GetAllForHomeAsync()
        {
            return await context.Sports
                .Where(s => !s.IsDeleted)
                .Select(s => new SportHomeViewModel
                {
                    Id = s.Id,
                    Name = s.Name,
                    ImageUrl = s.ImageUrl
                })
                .ToListAsync();
        }


        public async Task<SportReservationFormViewModel?> GetReservationFormAsync(int sportId, string? currentUserId = null)
        {
            var sport = await context.Sports
                .Include(s => s.Facility)
                .FirstOrDefaultAsync(s => s.Id == sportId);

            if (sport == null)
            {
                return null;
            }

            int? currentTrainerId = null;

            if (!string.IsNullOrEmpty(currentUserId))
            {
                currentTrainerId = await context.Trainers
                    .Where(t => t.ClientId == currentUserId && !t.IsDeleted)
                    .Select(t => (int?)t.Id)
                    .FirstOrDefaultAsync();
            }

            var now = time.GetLocalNow().DateTime;

            var model = new SportReservationFormViewModel
            {
                SportId = sport.Id,
                SportName = sport.Name,
                FacilityName = sport.Facility.Name,
                FacilityImageUrl = sport.Facility.ImageUrl,
                ReservationDateTime = now.AddDays(1),
                Duration = sport.Duration,
                MinDuration = sport.Duration,
                MaxDuration = sport.Duration * 2,
                MinPeople = sport.MinPeople,
                MaxPeople = sport.MaxPeople,
                Trainers = await context.SportTrainers
                    .Where(st => st.SportId == sportId && (currentTrainerId == null || st.TrainerId != currentTrainerId))
                    .Select(st => new TrainerDropdownViewModel
                    {
                        Id = st.TrainerId,
                        FullName = st.Trainer.Name + " " + st.Trainer.LastName,
                        ImageUrl = st.Trainer.ImageUrl,
                    })
                    .ToListAsync()
            };

            return model;
        }


        public async Task<int> CreateReservationAsync(SportReservationFormViewModel model, string userId)
        {
            var sport = await context.Sports
                .FirstOrDefaultAsync(s => s.Id == model.SportId && !s.IsDeleted);

            var reservationTime = model.ReservationDateTime;
            var now = time.GetLocalNow().DateTime;

            if (reservationTime.TimeOfDay < ApplicationConstants.OpeningTime
                || reservationTime.TimeOfDay >= ApplicationConstants.ClosingTime)
            {
                throw new InvalidOperationException(sharedLocalizer[ReservationOutsideWorkingHours]);
            }

            if(reservationTime > now.AddDays(ApplicationConstants.MaxReservationDaysInAdvance))
            {
                throw new InvalidOperationException(sharedLocalizer[ReservationTooFarInFuture]);
            }


            if (reservationTime < now)
            {
                throw new InvalidOperationException(sharedLocalizer[ReservationInPast]);
            }

            if (reservationTime < now.AddHours(1))
            {
                throw new InvalidOperationException(sharedLocalizer[ReservationTooSoon]);
            }

            var startTime = model.ReservationDateTime;
            var endTime = startTime.AddMinutes(model.Duration);
            var startDate = startTime.Date;

            bool isHired = await context.Reservations
                .AnyAsync(r =>
                    r.ClientId == userId &&
                    r.ReservationDateTime.Year == startTime.Year &&
                    r.ReservationDateTime.Month == startTime.Month &&
                    r.ReservationDateTime.Day == startTime.Day &&
                    r.ReservationDateTime < endTime &&
                    r.ReservationDateTime.AddMinutes(r.Duration) > startTime);

            if (isHired)
            {
                throw new InvalidOperationException(sharedLocalizer[ReservationConflict]);
            }

            if (model.TrainerId.HasValue)
            {
                bool trainerBusy = await context.Reservations
                    .AnyAsync(r =>
                        r.TrainerId == model.TrainerId &&
                        r.ReservationDateTime.Year == startTime.Year &&
                        r.ReservationDateTime.Month == startTime.Month &&
                        r.ReservationDateTime.Day == startTime.Day &&
                        r.ReservationDateTime < endTime &&
                        r.ReservationDateTime.AddMinutes(r.Duration) > startTime);

                if (trainerBusy)
                {
                    throw new InvalidOperationException(sharedLocalizer[TrainerBusy]);
                }
            }

            int? currentTrainerId = await context.Trainers
                .Where(t => t.ClientId == userId && !t.IsDeleted)
                .Select(t => (int?)t.Id)
                .FirstOrDefaultAsync();

            if (currentTrainerId.HasValue)
            {
                bool selfBusy = await context.Reservations.AnyAsync(r =>
                    r.TrainerId == currentTrainerId.Value &&
                    r.ReservationDateTime.Year == startTime.Year &&
                    r.ReservationDateTime.Month == startTime.Month &&
                    r.ReservationDateTime.Day == startTime.Day &&
                    r.ReservationDateTime < endTime &&
                    r.ReservationDateTime.AddMinutes(r.Duration) > startTime);

                if (selfBusy)
                    throw new InvalidOperationException(sharedLocalizer[ReservationConflict]);
            }

            bool spaOverlap = await context.SpaReservations
                .Include(sr => sr.SpaService)
                .AnyAsync(sr =>
                    sr.ClientId == userId &&
                    sr.ReservationDateTime.Year == startTime.Year &&
                    sr.ReservationDateTime.Month == startTime.Month &&
                    sr.ReservationDateTime.Day == startTime.Day &&
                    sr.ReservationDateTime < endTime &&
                    sr.ReservationDateTime.AddMinutes(sr.SpaService.Duration) > startTime);

            if (spaOverlap)
            {
                throw new InvalidOperationException(sharedLocalizer[ReservationConflict]);
            }

            var reservation = new Reservation
            {
                ClientId = userId,
                SportId = model.SportId,
                TrainerId = model.TrainerId,
                Duration = model.Duration,
                NumberOfPeople = model.NumberOfPeople,
                ReservationDateTime = reservationTime
            };

            await context.Reservations.AddAsync(reservation);
            await context.SaveChangesAsync();

            return reservation.Id;
        }

        public async Task<IEnumerable<SportReservationViewModel>> GetUserReservationsAsync(string userId)
        {
            return await context.Reservations
         .Where(r => r.ClientId == userId)

         .Where(r => r.ReservationDateTime >= DateTime.Now)

         .Include(r => r.Sport).ThenInclude(s => s.Facility)
         .Include(r => r.Trainer)

         .OrderBy(r => r.ReservationDateTime)

         .Select(r => new SportReservationViewModel
         {
             Id = r.Id,
             SportName = r.Sport.Name,
             FacilityName = r.Sport.Facility.Name,
             Duration = r.Duration,
             ReservationDateTime = r.ReservationDateTime,
             NumberOfPeople = r.NumberOfPeople,
             TrainerName = r.Trainer != null ? $"{r.Trainer.Name} {r.Trainer.LastName}" : "No Trainer",
             TotalPrice = Math.Round(r.Sport.Price * ((decimal)r.Duration / r.Sport.Duration) * r.NumberOfPeople, 2)
         })
         .ToListAsync();
        }

        public async Task<bool> ReservationExistsAsync(int reservationId, string userId)
        {
            return await context.Reservations
                .AnyAsync(r => r.Id == reservationId && r.ClientId == userId);
        }

        public async Task CancelReservationAsync(int reservationId, string userId)
        {
            var reservation = await context.Reservations
                .FirstOrDefaultAsync(r => r.Id == reservationId && r.ClientId == userId);
            if (reservation != null)
            {
                context.Reservations.Remove(reservation);
                await context.SaveChangesAsync();
            }
        }

        public async Task DeleteExpiredReservationsAsync(string userId)
        {
            await Task.CompletedTask;
        }


        //CRUD operations for sports
        public async Task<AddSportViewModel> GetAddFormModelAsync()
        {
            var facilities = await GetFacilitiesSelectListAsync();

            return new AddSportViewModel
            {
                Facilities = facilities
            };
        }

        public async Task AddAsync(AddSportViewModel model)
        {
            var sport = new Sport
            {
                Name = model.Name,
                Price = model.Price,
                Duration = model.Duration,
                ImageUrl = model.ImageUrl,
                FacilityId = model.FacilityId,
                MinPeople = model.MinPeople,
                MaxPeople = model.MaxPeople,
                IsDeleted = false
            };

            await context.Sports.AddAsync(sport);
            await context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(string name)
        {
            return await context.Sports
                .AnyAsync(s => s.Name == name && !s.IsDeleted);
        }

        public async Task<AddSportViewModel?> GetSportForEditAsync(int id)
        {
            var sport = await context.Sports
                .Where(s => s.Id == id && !s.IsDeleted)
                .FirstOrDefaultAsync();
            if (sport == null)
                return null;

            var facilities = await GetFacilitiesSelectListAsync();

            return new AddSportViewModel
            {
                Id = sport.Id,
                Name = sport.Name,
                Price = sport.Price,
                Duration = sport.Duration,
                ImageUrl = sport.ImageUrl,
                FacilityId = sport.FacilityId,
                MinPeople = sport.MinPeople,
                MaxPeople = sport.MaxPeople,
                Facilities = facilities,
            };
        }

        public async Task EditAsync(int id, AddSportViewModel model)
        {
            var sport = await context.Sports.FindAsync(id);
            if (sport == null)
                return;

            sport.Name = model.Name;
            sport.Price = model.Price;
            sport.Duration = model.Duration;
            sport.ImageUrl = model.ImageUrl;
            sport.FacilityId = model.FacilityId;
            sport.MinPeople = model.MinPeople;
            sport.MaxPeople = model.MaxPeople;

            await context.SaveChangesAsync();
        }

        public async Task<DeleteSportViewModel?> GetSportForDeleteAsync(int id)
        {
            var sport = await context.Sports
                .Where(s => s.Id == id && !s.IsDeleted)
                .Include(s => s.Facility)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (sport == null)
                return null;

            return new DeleteSportViewModel
            {
                Id = sport.Id,
                Name = sport.Name,
                Facility = sport.Facility.Name
            };
        }

        public async Task DeleteAsync(int id)
        {
            var sport = await context.Sports
                .FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted);
            if (sport != null)
            {
                sport.IsDeleted = true;
                await context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<SelectListItem>> GetFacilitiesSelectListAsync()
        {
            return await context.Facilities
                .Where(f => !f.IsDeleted)
                .Select(f => new SelectListItem
                {
                    Value = f.Id.ToString(),
                    Text = f.Name
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<SelectListItem>> GetAllAsSelectListAsync()
        {
            return await context.Sports
                .Where(s => !s.IsDeleted)
                .OrderBy(s => s.Name)
                .Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = s.Name
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<SportReportViewModel>> GetSportReservationsReportAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = context.Reservations
        .IgnoreQueryFilters()
        .Where(r => r.ReservationDateTime < DateTime.Now)
        .Include(r => r.Sport)
        .Include(r => r.Client)
        .AsQueryable();

            if (startDate.HasValue)
            {
                query = query.Where(r => r.ReservationDateTime.Date >= startDate.Value.Date);
            }
            if (endDate.HasValue)
            {
                query = query.Where(r => r.ReservationDateTime.Date <= endDate.Value.Date);
            }

            return await query
                .OrderByDescending(r => r.ReservationDateTime)
                .Select(r => new SportReportViewModel
                {
                    ClientName = r.Client != null ? $"{r.Client.FirstName} {r.Client.LastName}" : "Изтрит профил",
                    SportName = r.Sport != null ? r.Sport.Name : "Изтрит спорт",
                    Date = r.ReservationDateTime.ToString("dd.MM.yyyy HH:mm"),
                    Price = r.Sport != null ? (r.Sport.Price * r.NumberOfPeople) : 0m,
                    IsSportDeleted = r.Sport == null
                })
                .ToListAsync();
        }
    }
}

