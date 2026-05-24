using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportComplexApp.Data.Models
{
    public class TrainerSession : BaseEntity
    {
        public int Id { get; set; }

        [Required]
        public int TrainerId { get; set; }
        public Trainer Trainer { get; set; } = null!;

        [Required]
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}
