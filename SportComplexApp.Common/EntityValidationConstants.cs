using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportComplexApp.Common
{
    public static class EntityValidationConstants
    {
        public static class Sport
        {
            public const int NameMaxLength = 100;
            public const int NameMinLength = 2;

            public const int ImageUrlMaxLength = 500;

            public const int DurationMinValue = 30; // in minutes
            public const int DurationMaxValue = 180; // in minutes

            public const decimal PriceMinValue = 0.00m; // in currency
            public const decimal PriceMaxValue = 1000.00m; // in currency
        }

        public static class Trainer
        {
            public const int NameMaxLength = 100;
            public const int NameMinLength = 2;

            public const int SpezializationMaxLength = 100;
            public const int SpezializationMinLength = 5;

            public const int BioMaxLenght = 500;
            public const int BioMinLength = 10;

            public const int ImageUrlMaxLength = 500;
        }

        public static class Client
        {
            public const int FirstNameMaxLength = 50;
            public const int LastNameMaxLength = 50;
        }

        public static class Facility
        {
            public const int NameMaxLength = 100;
            public const int NameMinLength = 2;
        }

        public static class Reservation
        {
            public const int NumberOfPeopleMinValue = 1;
            public const int NumberOfPeopleMaxValue = 30;
            public const int DurationMinValue = 15;
            public const int DurationMaxValue = 300;
            public const string ReservationDateTimeFormat = "{0:yyyy-MM-ddTHH:mm}";

        }

        public static class  SpaReservation
        {
            public const int MinPeopleValue = 1;
            public const int MaxPeopleValue = 50;
            public const string ReservationDateTimeFormat = "{0:yyyy-MM-ddTHH:mm}";
        }

        public static class SpaService
        {
            public const int NameMaxLength = 100;
            public const int NameMinLength = 3;
            public const int DescriptionMaxLength = 1000;
            public const int DescriptionMinLength = 10;
            public const decimal PriceMinValue = 5.00m; // in currency
            public const decimal PriceMaxValue = 1000.00m; // in currency
            public const int ImageUrlMaxLength = 500;
        }

        public static class Tournament
        {
            public const int NameMaxLength = 100;
            public const int DescriptionMaxLength = 1000;
        }
    }
}
