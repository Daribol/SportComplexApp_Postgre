using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportComplexApp.Data.Models
{
    public class Log_22180008
    {
        public int Id { get; set; }
        public string TableName { get; set; } = null!;
        public string OperationType { get; set; } = null!;
        public DateTime OperationDate { get; set; }
    }
}
