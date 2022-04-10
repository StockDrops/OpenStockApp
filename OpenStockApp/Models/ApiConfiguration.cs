using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenStockApp.Models
{
    public class ApiConfiguration
    {
        public string? ApiBaseUrl { get; set; }
        public string? CertificatePath { get; set; }
        public string? CertificateThumbprint { get; set; }
        public string? CertificatePublicKey { get; set; }
        public string? CertificatePrivateKey { get; set; }
    }
}
