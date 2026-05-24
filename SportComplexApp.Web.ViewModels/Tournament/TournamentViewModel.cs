using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportComplexApp.Web.ViewModels.Tournament
{
    public class TournamentViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Sport { get; set; } = null!;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Description { get; set; } = null!;
        public string? ImageUrl { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}
