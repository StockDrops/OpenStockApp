using OpenStockApi.Core.Models.Searches;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenStockApp.Core.Contracts.Services.Settings
{
    public interface IFilterService
    {
        /// <summary>
        /// This method allows the app to know if a notification should be shown or not to the user based on his settings.
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        bool CanShowNotification(Result result);
    }
}
