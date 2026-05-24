using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportComplexApp.Web.ViewModels.Home
{
    public class TrainerHomeViewModel
    {
        public int Id { get; set; }
        public string FullName { get; set; } = null!;
        public string ImageUrl { get; set; } = null!;
    }
}
