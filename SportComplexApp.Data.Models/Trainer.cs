using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportComplexApp.Data.Models
{
    public class Trainer : BaseEntity
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public string? Bio { get; set; }

        public string? ImageUrl { get; set; }

        public bool IsDeleted { get; set; } = false;

        public string? ClientId { get; set; } = null!;
        public Client Client { get; set; } = null!;

        public virtual ICollection<TrainerSession> TrainerSessions { get; set; } = new HashSet<TrainerSession>();
        public virtual ICollection<SportTrainer> SportTrainers { get; set; } = new HashSet<SportTrainer>();
        public virtual ICollection<Reservation> Reservations { get; set; } = new HashSet<Reservation>();
    }
}
