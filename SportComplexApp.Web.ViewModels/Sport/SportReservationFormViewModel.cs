using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SportComplexApp.Common.EntityValidationConstants.Reservation;

namespace SportComplexApp.Web.ViewModels.Sport
{
    public class SportReservationFormViewModel
    {
        public int SportId { get; set; }

        public string SportName { get; set; } = null!;

        public string FacilityName { get; set; } = null!;
        public string? FacilityImageUrl { get; set; }

        [Required]
        [Display(Name = "LabelReservationDateTime")]
        [DisplayFormat(DataFormatString = ReservationDateTimeFormat, ApplyFormatInEditMode = true)]
        public DateTime ReservationDateTime { get; set; }

        [Required]
        [Range(DurationMinValue, DurationMaxValue)]
        public int Duration { get; set; }

        public int MinDuration { get; set; }
        public int MaxDuration { get; set; }

        [Required]
        [Display(Name = "LabelNumberOfPeople")]
        public int NumberOfPeople { get; set; }

        public int MinPeople { get; set; }
        public int MaxPeople { get; set; }

        public int? TrainerId { get; set; }

        public IEnumerable<TrainerDropdownViewModel> Trainers { get; set; } = new List<TrainerDropdownViewModel>();
    }
}
