using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using static SportComplexApp.Common.EntityValidationConstants.SpaReservation;
using static SportComplexApp.Common.ErrorMessages.SpaReservation;

namespace SportComplexApp.Web.ViewModels.Spa
{
    public class SpaReservationFormViewModel
    {
        [Required(ErrorMessage = "RequiredFieldError")]
        public int SpaServiceId { get; set; }
        public string SpaServiceName { get; set; } = null!;
        public string? ImageUrl { get; set; }

        [Required(ErrorMessage = "RequiredFieldError")]
        [DataType(DataType.DateTime)]
        [Display(Name = "LabelReservationDateTime")]
        [DisplayFormat(DataFormatString = ReservationDateTimeFormat, ApplyFormatInEditMode = true)]
        public DateTime ReservationDate { get; set; }

        [Required(ErrorMessage = "RequiredFieldError")]
        [Display(Name = "LabelNumberOfPeople")]
        [Range(1, 10, ErrorMessage = NumberOfPeopleOutOfRange)]
        public int NumberOfPeople { get; set; }
    }
}
