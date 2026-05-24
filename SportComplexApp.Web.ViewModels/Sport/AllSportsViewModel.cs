using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportComplexApp.Web.ViewModels.Sport
{
    public class AllSportsViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string ImageUrl { get; set; } = null!;
        public int Duration { get; set; }
        public decimal Price { get; set; }
        public string Facility { get; set; } = null!;
    }
}
