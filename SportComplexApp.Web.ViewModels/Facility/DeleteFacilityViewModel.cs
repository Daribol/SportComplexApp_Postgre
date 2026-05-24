using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportComplexApp.Web.ViewModels.Facility
{
    public class DeleteFacilityViewModel
    {
        public int Id { get; set; }

        [Display(Name = "LabelFacilityName")]
        public string Name { get; set; } = null!;
    }
}
