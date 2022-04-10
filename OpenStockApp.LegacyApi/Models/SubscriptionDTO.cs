using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenStockApp.LegacyApi.Models
{
    public class SubscriptionDTO
    {
        /// <summary>
        /// Subscription name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The alert limit
        /// </summary>
        public int AlertLimit { get; set; }
        /// <summary>
        /// Any added delay to alerts - usually 0.
        /// </summary>
        public int Delay { get; set; } //in milliseconds
    }
}
