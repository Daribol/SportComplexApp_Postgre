using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportComplexApp.Web.ViewModels.Spa
{
    public class PaginationSpaServiceViewModel
    {
        public IEnumerable<SpaServiceViewModel> SpaServices { get; set; } = new List<SpaServiceViewModel>();

        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public string? SearchQuery { get; set; }
        public string? SortBy { get; set; }
        public int? MinDuration { get; set; }
        public int? MaxDuration { get; set; }
    }
}
