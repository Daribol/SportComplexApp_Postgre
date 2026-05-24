using SportComplexApp.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportComplexApp.Web.ViewModels.Trainer
{
    public class DropdownTrainerViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public ICollection<SportTrainer> SportTrainers { get; set; }
    }
}
