using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenStockApp.Email.Models.Email
{
    public class EmailBody
    {
        public string? Content { get; set; }
        public BodyType BodyType { get; set; }
    }
}
