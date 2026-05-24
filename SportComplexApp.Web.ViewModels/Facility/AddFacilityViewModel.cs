using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using static SportComplexApp.Common.EntityValidationConstants.Facility;

namespace SportComplexApp.Web.ViewModels.Facility
{
    public class AddFacilityViewModel
    {
        public int Id { get; set; }
        [Required]
        [StringLength(NameMaxLength, MinimumLength = NameMinLength)]
        [Display(Name = "LabelFacilityName")]
        public string Name { get; set; } = null!;
        public string? ImageUrl { get; set; }
    }
}
