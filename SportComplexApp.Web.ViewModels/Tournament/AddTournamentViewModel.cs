using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportComplexApp.Web.ViewModels.Tournament
{
    public class AddTournamentViewModel
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = null!;

        [Required]
        [Display(Name = "LabelDescription")]
        public string Description { get; set; } = null!;

        [Display(Name = "LabelStartDate")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Display(Name = "LabelEndDate")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        [Display(Name = "LabelSport")]
        public int SportId { get; set; }
        public string? ImageUrl { get; set; }

        public IEnumerable<SelectListItem> Sports { get; set; } = new List<SelectListItem>();
    }
}
