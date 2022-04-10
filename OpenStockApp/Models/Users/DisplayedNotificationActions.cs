using OpenStockApi.Core.Models.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenStockApp.Models.Users
{
    public class DisplayedNotificationActions
    {
        public string Description { get; set; }
        public NotificationAction Action { get; set; }
    }
}
