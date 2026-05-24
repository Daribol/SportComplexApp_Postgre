using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportComplexApp.Web.ViewModels.Sport
{
    public class SportReservationViewModel
    {
        public int Id { get; set; }

        public string SportName { get; set; } = null!;

        public string FacilityName { get; set; } = null!;

        public DateTime ReservationDateTime { get; set; }

        public int Duration { get; set; }

        public int NumberOfPeople { get; set; }

        public string? TrainerName { get; set; }

        public decimal TotalPrice { get; set; }
    }
}
