using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportComplexApp.Web.ViewModels.Trainer
{
    public class TrainerReservationViewModel
    {
        public int Id { get; set; }
        public string ClientName { get; set; } = null!;
        public string SportName { get; set; } = null!;
        public DateTime ReservationDate { get; set; }
        public int Duration { get; set; }
        public int NumberOfPeople { get; set; }
        public string FacilityName { get; set; } = null!;
    }
}
