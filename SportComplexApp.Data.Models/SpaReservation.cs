using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportComplexApp.Data.Models
{
    public class SpaReservation :BaseEntity
    {
        public int Id { get; set; }

        public string ClientId { get; set; } = null!;
        public Client Client { get; set; } = null!;

        public int SpaServiceId { get; set; }
        public SpaService SpaService { get; set; } = null!;

        public DateTime ReservationDateTime { get; set; }

        public int NumberOfPeople { get; set; }
    }
}
