using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportComplexApp.Data.Models
{
    public class Reservation :BaseEntity
    {
        public int Id { get; set; }

        public string ClientId { get; set; } = null!;
        public Client Client { get; set; } = null!;

        public int SportId { get; set; }
        public Sport Sport { get; set; } = null!;

        public int? TrainerId { get; set; }
        public Trainer? Trainer { get; set; }

        public DateTime ReservationDateTime { get; set; }

        public int Duration { get; set; }

        public int NumberOfPeople { get; set; }
    }
}
