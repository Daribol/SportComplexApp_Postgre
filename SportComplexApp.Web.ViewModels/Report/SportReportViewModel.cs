using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportComplexApp.Web.ViewModels.Report
{
    public class SportReportViewModel
    {
        public string ClientName { get; set; } = null!;
        public string SportName { get; set; } = null!;
        public string Date { get; set; } = null!;
        public decimal Price { get; set; }
        public bool IsSportDeleted { get; set; }
    }
}
