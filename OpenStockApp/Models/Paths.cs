using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenStockApp.Models
{
    public class Paths
    {
        public const string LogFile =
#if WINDOWS
            "StockDrops\\logs\\applog.txt";
#else
            "StockDrops/logs/applog.txt";
#endif
    }
}
