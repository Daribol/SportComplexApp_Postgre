using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportComplexApp.Data.Models
{
    public class Tournament : BaseEntity
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public string Description { get; set; } = null!;

        public int SportId { get; set; }
        public Sport Sport { get; set; } = null!;
        public string? ImageUrl { get; set; }

        public bool IsDeleted { get; set; } = false;

        public virtual ICollection<TournamentRegistration> TournamentRegistrations { get; set; } = new HashSet<TournamentRegistration>();
    }
}
