using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportComplexApp.Web.ViewModels.Admin
{
    public class AllUsersViewModel
    {
        public string Id { get; set; } = null!;
        public string? Email { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }
        public IEnumerable<string> Roles { get; set; } = null!;
    }
}
