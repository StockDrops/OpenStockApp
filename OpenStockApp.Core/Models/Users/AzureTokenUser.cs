using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenStockApp.Core.Models.Users
{

    public class AzureTokenUser
    {
        public string? ver { get; set; }
        public string? iss { get; set; }
        public string? sub { get; set; }
        public string? aud { get; set; }
        public int exp { get; set; }
        public int iat { get; set; }
        public int nbf { get; set; }
        public string? name { get; set; }
        public string? preferred_username { get; set; }
        public string? oid { get; set; }
        public string? tid { get; set; }
        public string? aio { get; set; }
    }
}
