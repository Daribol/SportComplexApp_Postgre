using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using static SportComplexApp.Common.ErrorMessages.Sport;

namespace SportComplexApp.Web.ViewModels.Sport
{
    public class AddSportViewModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; } = null!;

        [Required]
        [Range(5.00, 1000)]
        public decimal Price { get; set; }

        public string? ImageUrl { get; set; }

        [Required]
        [Range(15, 300)]
        public int Duration { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = MinNumberOfPeople)]
        public int MinPeople { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = MaxNumberOfPeople)]
        public int MaxPeople { get; set; }

        [Required]
        [Display(Name = "Facility")]
        public int FacilityId { get; set; }

        [JsonIgnore]
        [BindNever]
        public IEnumerable<SelectListItem> Facilities { get; set; } = new List<SelectListItem>();
    }
}
