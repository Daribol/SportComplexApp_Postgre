using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportComplexApp.Web.ViewModels.Tournament
{
    public class DeleteTournamentViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string Sport { get; set; } = null!;
    }
}
