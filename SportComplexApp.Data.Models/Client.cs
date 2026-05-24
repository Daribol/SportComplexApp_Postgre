using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportComplexApp.Data.Models
{
    public class Client : IdentityUser
    {
        public DateTime LastModified_22180008 { get; set; }
        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public virtual ICollection<Reservation> Reservations { get; set; } = new HashSet<Reservation>();
        public virtual ICollection<SpaReservation> SpaReservations { get; set; } = new HashSet<SpaReservation>();
        public virtual ICollection<TournamentRegistration> TournamentRegistrations { get; set; } = new HashSet<TournamentRegistration>();
    }
}
