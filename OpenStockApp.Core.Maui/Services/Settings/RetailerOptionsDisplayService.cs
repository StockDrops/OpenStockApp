using OpenStockApi.Core.Models.Regions;
using OpenStockApi.Core.Models.Users;
using OpenStockApp.Core.Contracts.Services.Hubs;
using OpenStockApp.Core.Contracts.Services.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenStockApp.Core.Maui.Services.Settings
{
    public class RetailerOptionsDisplayService : IRetailerOptionsDisplayService
    {

        public event EventHandler<RetailerOptions>? DisplayRetailerOptions;

        private readonly IUserOptionsService _userOptionsService;
        private readonly IUserOptionsHubClient _userHubClient;
        public RetailerOptionsDisplayService(IUserOptionsService userOptionsService, IUserOptionsHubClient userOptionsHubClient)
        {
            _userOptionsService = userOptionsService;
            _userHubClient = userOptionsHubClient;
        }

        /// <summary>
        /// Loads all the retailers for a given country
        /// </summary>
        /// <param name="country"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task LoadAndDisplayRetailers(Country country, CancellationToken token = default)
        {
            var retailers = await _userHubClient.GetRetailersByCountry(country, token);
            foreach (var retailer in retailers)
            {
                var options = _userOptionsService
                    .UserOptions?
                    .RetailerOptions
                    .Where(r => r.RetailerId == retailer.Id)
                    .FirstOrDefault();

                if (options is not null && options.Retailer == null)
                {
                    options.Retailer = retailer;
                    DisplayRetailerOptions?.Invoke(this, options);
                }
                else if (options is not null)
                {
                    DisplayRetailerOptions?.Invoke(this, options);
                }
                else if (options is null)
                {
                    //in this case there's no options in the user settings we must create one.
                    options = new RetailerOptions
                    {
                        Id = 0, //new options must have id = 0
                        IsEnabled = false,
                        RetailerId = retailer.Id,
                        Retailer = retailer

                    };

                    //the options must be added to the user options:
                    _userOptionsService
                        .UserOptions?
                        .RetailerOptions
                        .Add(options);

                    DisplayRetailerOptions?.Invoke(this, options);
                }
            }
        }



    }
}
