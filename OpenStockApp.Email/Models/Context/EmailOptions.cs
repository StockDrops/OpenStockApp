using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenStockApp.Email.Models.Context
{
    public class EmailOptions
    {
        public string? EncryptionKey { get; set; }
        public string? ConnectionString { get; set; }
        public string? DatabaseName { get; set; }

    }
}
