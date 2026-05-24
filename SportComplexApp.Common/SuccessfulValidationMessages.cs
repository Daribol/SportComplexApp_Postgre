namespace SportComplexApp.Common
{
    public static class SuccessfulValidationMessages
    {
        public static class Users
        {
            public const string RoleAssigned = nameof(RoleAssigned);
            public const string RoleRemoved = nameof(RoleRemoved);
            public const string UserDeleted = nameof(UserDeleted);
        }
        public static class Sport
        {
            public const string SportAddedToMyList = nameof(SportAddedToMyList);
            public const string SportRemovedFromMyList = nameof(SportRemovedFromMyList);

            public const string SportCreated = nameof(SportCreated);
            public const string SportUpdated = nameof(SportUpdated);
            public const string SportDeleted = nameof(SportDeleted);
        }

        public static class Reservation
        {
            public const string ReservationCreated = nameof(ReservationCreated);
            public const string ReservationDeleted = nameof(ReservationDeleted);
        }

        public static class SpaService
        {
            public const string SpaServiceCreated = nameof(SpaServiceCreated);
            public const string SpaServiceUpdated = nameof(SpaServiceUpdated);
            public const string SpaServiceDeleted = nameof(SpaServiceDeleted);
        }
        public static class SpaReservation
        {
            public const string SpaReservationCreated = nameof(SpaReservationCreated);
            public const string SpaReservationDeleted = nameof(SpaReservationDeleted);
        }

        public static class Tournament
        {
            public const string TournamentRegistered = nameof(TournamentRegistered);
            public const string TournamentUnregistered = nameof(TournamentUnregistered);
            public const string TournamentCreated = nameof(TournamentCreated);
            public const string TournamentUpdated = nameof(TournamentUpdated);
            public const string TournamentDeleted = nameof(TournamentDeleted);
        }

        public static class Trainer
        {
            public const string TrainerAdded = nameof(TrainerAdded);
            public const string TrainerUpdated = nameof(TrainerUpdated);
            public const string TrainerDeleted = nameof(TrainerDeleted);
        }

        public static class Facility
        {
            public const string FacilityAdded = nameof(FacilityAdded);
            public const string FacilityUpdated = nameof(FacilityUpdated);
            public const string FacilityDeleted = nameof(FacilityDeleted);
        }
    }
}
