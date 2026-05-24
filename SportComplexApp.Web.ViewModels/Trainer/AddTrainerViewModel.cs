using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SportComplexApp.Common.EntityValidationConstants.Trainer;

namespace SportComplexApp.Web.ViewModels.Trainer
{
    public class AddTrainerViewModel
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(NameMaxLength)]
        [MinLength(NameMinLength)]
        public string Name { get; set; } = null!;

        [Required]
        [MaxLength(NameMaxLength)]
        [MinLength(NameMinLength)]
        public string LastName { get; set; } = null!;

        [MaxLength(BioMaxLenght)]
        [MinLength(BioMinLength)]
        public string Bio { get; set; } = null!;

        public string? ImageUrl { get; set; }

        public List<int> SelectedSportIds { get; set; } = new List<int>();

        public IEnumerable<SelectListItem> AvailableSports { get; set; } = new List<SelectListItem>();

    }
}
