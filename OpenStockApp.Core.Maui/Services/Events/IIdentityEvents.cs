using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenStockApp.Core.Maui.Services.Events
{
    /// <summary>
    /// Basic indenty events.
    /// </summary>
    public interface IIdentityEvents
    {
        /// <summary>
        /// Event to be invoked when the user logs in.
        /// </summary>
        event EventHandler LoggedIn;
        /// <summary>
        /// Event to be invoked when the user logs out.
        /// </summary>
        event EventHandler LoggedOut;
    }
}
