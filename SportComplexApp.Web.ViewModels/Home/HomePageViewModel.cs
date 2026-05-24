using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportComplexApp.Web.ViewModels.Home
{
    public class HomePageViewModel
    {
        public IEnumerable<SportHomeViewModel> Sports { get; set; } = new List<SportHomeViewModel>();
        public IEnumerable<TrainerHomeViewModel> Trainers { get; set; } = new List<TrainerHomeViewModel>();
        public IEnumerable<SpaProcedureHomeViewModel> SpaProcedures { get; set; } = new List<SpaProcedureHomeViewModel>();
    }
}
