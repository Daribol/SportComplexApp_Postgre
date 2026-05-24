using SportComplexApp.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportComplexApp.Web.ViewModels.Trainer
{
    public class TrainerDetailsViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public List<string> Sports { get; set; } = new();
        public string? Bio { get; set; }
        public string? ImageUrl { get; set; }

        public int CurrentMonth { get; set; }
        public int CurrentYear { get; set; }
        public List<TrainerReservationViewModel> Reservations { get; set; } = new();
    }
}
