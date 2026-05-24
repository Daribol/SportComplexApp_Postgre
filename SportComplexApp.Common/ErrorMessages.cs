using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SportComplexApp.Common
{
    public static class ErrorMessages
    {
        public static class Users
        {
            public const string UserIdOrRoleCannotBeEmpty = nameof(UserIdOrRoleCannotBeEmpty);
            public const string UserDoesNotExist = nameof(UserDoesNotExist);
            public const string FailedToAssignRole = nameof(FailedToAssignRole);
            public const string FailedToRemoveRole = nameof(FailedToRemoveRole);
            public const string FailedToDeleteUser = nameof(FailedToDeleteUser);
            public const string UserAlreadyInRole = nameof(UserAlreadyInRole);
        }

        public static class Sport
        {
            public const string MinNumberOfPeople = nameof(MinNumberOfPeople);
            public const string MaxNumberOfPeople = nameof(MaxNumberOfPeople);
            public const string MaxPeopleLessThanMin = nameof(MaxPeopleLessThanMin);
            public const string SportAlreadyExists = nameof(SportAlreadyExists);
            public const string SportNotFound = nameof(SportNotFound);
        }

        public static class Reservation
        {
            public const string ReservationTooSoon = nameof(ReservationTooSoon);
            public const string ReservationInPast = nameof(ReservationInPast);

            public const string ReservationConflict = nameof(ReservationConflict);
            public const string TrainerBusy = nameof(TrainerBusy);
            public const string ReservationOutsideWorkingHours = nameof(ReservationOutsideWorkingHours);
            public const string ReservationTooFarInFuture = nameof(ReservationTooFarInFuture);
        }

        public static class  SpaReservation
        {
            public const string ReservationInPast = nameof(ReservationInPast);
            public const string ReservationTooSoon = nameof(ReservationTooSoon);
            public const string ReservationConflict = nameof(ReservationConflict);
            public const string NumberOfPeopleOutOfRange = nameof(NumberOfPeopleOutOfRange);
            public const string ReservationOutsideWorkingHours = nameof(ReservationOutsideWorkingHours);
            public const string ReservationTooFarInFuture = nameof(ReservationTooFarInFuture);
        }

        public static class Tournament
        {
            public const string TournamentAlreadyRegistered = nameof(TournamentAlreadyRegistered);
            public const string TournamentRegistrationClosed = nameof(TournamentRegistrationClosed);

            public const string CannotUnregister = nameof(CannotUnregister);
            public const string TrainerCannotRegister = nameof(TrainerCannotRegister);

            public const string TournamentStartInPast = nameof(TournamentStartInPast);
            public const string TournamentEndBeforeStart = nameof(TournamentEndBeforeStart);
            public const string TournamentAlreadyExists = nameof(TournamentAlreadyExists);
            public const string TournamentNotFound = nameof(TournamentNotFound);
        }

        public static class Trainer
        {
            public const string TrainerAlreadyExists = nameof(TrainerAlreadyExists);
            public const string MustSelectAtLeastOneSport = nameof(MustSelectAtLeastOneSport);
            public const string TrainerNotFound = nameof(TrainerNotFound);
        }

        public static class Facility
        {
            public const string FacilityAlreadyExists = nameof(FacilityAlreadyExists);
            public const string FacilityHasSports = nameof(FacilityHasSports);
            public const string FacilityNotFound = nameof(FacilityNotFound);
        }

        public static class SpaService
        {
            public const string NameRequirenments = nameof(NameRequirenments);
            public const string DescriptionRequirenments = nameof(DescriptionRequirenments);
            public const string PriceTooLow = nameof(PriceTooLow);
            public const string SpaServiceNotFound = nameof(SpaServiceNotFound);
            public const string SpaServiceAlreadyExists = nameof(SpaServiceAlreadyExists);
        }
    }
}
