using SportComplexApp.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportComplexApp.Web.ViewModels.Trainer
{
    public class AllTrainersViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string? ImageUrl { get; set; }
        public List<string> Sports { get; set; } = new();

        public IEnumerable<TrainerReservationDetailViewModel> Reservations { get; set; } = new List<TrainerReservationDetailViewModel>();
    }
}
