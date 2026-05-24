using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportComplexApp.Data.Models
{
    public class Sport : BaseEntity
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public int FacilityId { get; set; }
        public Facility? Facility { get; set; } = null!;

        public decimal Price { get; set; }

        public string? ImageUrl { get; set; }

        public int Duration { get; set; }

        public int MinPeople { get; set; }
        public int MaxPeople { get; set; }

        public bool IsDeleted { get; set; } = false;

        public virtual ICollection<Reservation> Reservations { get; set; } = new HashSet<Reservation>();
        public virtual ICollection<SportTrainer> SportTrainers { get; set; } = new HashSet<SportTrainer>();
    }
}
