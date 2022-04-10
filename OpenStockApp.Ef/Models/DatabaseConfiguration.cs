using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenStockApp.Ef.Models
{
    public class DatabaseConfiguration
    {
        public string? ConnectionString { get; set; }
        public string? DatabaseName { get; set; }
    }
}
