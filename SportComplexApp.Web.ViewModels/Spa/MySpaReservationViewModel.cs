using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportComplexApp.Web.ViewModels.Spa
{
    public class MySpaReservationViewModel
    {
        public int Id { get; set; }

        public string SpaServiceName { get; set; } = null!;

        public DateTime DateTime { get; set; }

        public int Duration { get; set; }

        public int People { get; set; }

        public decimal TotalPrice { get; set; }
    }
}
