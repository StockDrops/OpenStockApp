using OpenStockApi.Core.Models.Searches;
using OpenStockApi.Core.Models.Users;
using OpenStockApp.Core.Contracts.Services.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenStockApp.Core.Maui.Services.Settings
{
    public class FilterService : IFilterService
    {
        private readonly IUserOptionsService userOptionService;
        public FilterService(IUserOptionsService userOptionService)
        {
            this.userOptionService = userOptionService;
        }

        public bool CanShowNotification(Result result)
        {
            if(result.Sku != null)
            {
                var modelOptions = userOptionService.GetModelOptionsFromCache(result.Sku.ModelId);
                if(modelOptions != null && modelOptions.IsEnabled)
                {
                    return IsResultInPriceLimits(modelOptions, result);
                }
            }
            return false;
        }
        /// <summary>
        /// Checks for the price limits of a result
        /// </summary>
        /// <param name="modelOptions"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        private bool IsResultInPriceLimits(ModelOptions modelOptions, Result result)
        {
            if(result.Price != null)
            {
                var ok = true;
                if(modelOptions.MinPrice != null)
                {
                    ok &= modelOptions.MinPrice <= result.Price.Value;
                }
                if(modelOptions.MaxPrice != null)
                {
                    ok &= modelOptions.MaxPrice >= result.Price.Value;
                }
                return ok; //If no price filters are set, and the min price and max price are null we will return ok (true)
            }
            return true; //IF no price is available we return true, and ok the notification.
        }
    }
}
