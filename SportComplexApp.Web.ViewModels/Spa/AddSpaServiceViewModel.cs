using System.ComponentModel.DataAnnotations;
using static SportComplexApp.Common.ErrorMessages.SpaService;
using static SportComplexApp.Common.EntityValidationConstants.SpaService;

namespace SportComplexApp.Web.ViewModels.Spa
{
    public class AddSpaServiceViewModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(NameMaxLength, MinimumLength = NameMinLength, ErrorMessage = NameRequirenments)]
        [Display(Name = "LabelSpaServiceName")]
        public string Name { get; set; } = null!;

        [Required]
        [StringLength(DescriptionMaxLength, MinimumLength = DescriptionMinLength, ErrorMessage = DescriptionRequirenments)]
        [Display(Name = "LabelDescription")]
        public string Description { get; set; } = null!;

        [Required]
        [Display(Name = "LabelProcedureDetails")]
        public string ProcedureDetails { get; set; } = null!;

        [Required]
        [Range(5.00, 1000.00, ErrorMessage = PriceTooLow)]
        [Display(Name = "LabelPrice")]
        public decimal Price { get; set; }

        [Required]
        [Range(20, 120)]
        [Display(Name = "LabelDurationMinutes")]
        public int Duration { get; set; }

        [Display(Name = "LabelImageUrl")]
        public string ImageUrl { get; set; } = null!;
    }
}
