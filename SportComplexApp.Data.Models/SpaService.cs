using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportComplexApp.Data.Models
{
    public class SpaService :BaseEntity
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string Description { get; set; } = null!;

        public string ProcedureDetails { get; set; } = null!;

        public decimal Price { get; set; }

        public string? ImageUrl { get; set; }

        public int Duration { get; set; }

        public bool IsDeleted { get; set; } = false;

        public virtual ICollection<SpaReservation> SpaReservations { get; set; } = new HashSet<SpaReservation>();
    }
}
