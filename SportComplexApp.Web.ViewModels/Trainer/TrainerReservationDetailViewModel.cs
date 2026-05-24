using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportComplexApp.Web.ViewModels.Trainer
{
    public class TrainerReservationDetailViewModel
    {
        public int Id { get; set; }

        public string CustomerName { get; set; } = null!;

        public string ReservationDate { get; set; } = null!;

        public string TimeSlot { get; set; } = null!;

        public string Status { get; set; } = null!;
    }
}
