using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenStockApp.Models
{
    public class AppConfig
    {
        public static bool Desktop
        {
            get
            {
#if WINDOWS || MACCATALYST
            return true;
#else
                return false;
#endif
            }
        }
        public string? PrivacyStatement { get; set; }
        public string? UserPortal { get; set; }
        public string? LicenseAgreement { get; set; }
        
    }
}
