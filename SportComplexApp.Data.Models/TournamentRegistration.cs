using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportComplexApp.Data.Models
{
    public class TournamentRegistration : BaseEntity
    {
        public int Id { get; set; }

        [Required]
        public string ClientId { get; set; } = null!;
        public Client Client { get; set; } = null!;

        [Required]
        public int TournamentId { get; set; }
        public Tournament Tournament { get; set; } = null!;
    }
}
