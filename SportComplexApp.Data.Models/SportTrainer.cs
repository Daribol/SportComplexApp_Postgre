using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportComplexApp.Data.Models
{
    public class SportTrainer :BaseEntity
    {
        public int SportId { get; set; }
        public Sport Sport { get; set; } = null!;

        public int TrainerId { get; set; }
        public Trainer Trainer { get; set; } = null!;
    }
}
