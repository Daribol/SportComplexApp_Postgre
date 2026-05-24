using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportComplexApp.Web.ViewModels.Facility
{
    public class FacilityMasterViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;
        public string? ImageUrl { get; set; }

        public IEnumerable<FacilitySportDetailViewModel> Sports { get; set; } = new List<FacilitySportDetailViewModel>();
    }
}
