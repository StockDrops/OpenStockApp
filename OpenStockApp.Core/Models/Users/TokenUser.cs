using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenStockApp.Core.Models.Users
{
    public class TokenUser
    {
        public int Exp { get; set; }
        public int Nbf { get; set; }
        public string? Ver { get; set; }
        public string? Iss { get; set; }
        public string? Sub { get; set; }
        public string? Aud { get; set; }
        public int Iat { get; set; }
        public int Auth_time { get; set; }
        public string? Oid { get; set; }
        public string? Given_name { get; set; }
        public string? Family_name { get; set; }
        public string? Name { get; set; }
        public string[]? Emails { get; set; }
        public string? Tfp { get; set; }
        public string? At_hash { get; set; }
    }
}
