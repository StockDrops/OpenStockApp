using Microsoft.Extensions.Logging;
using OpenStockApi.Core.Models.Users;
using OpenStockApp.Core.Contracts.Services.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenStockApp.Core.Services
{

    public class SubscriptionService : ISubscriptionService
    {
        //private readonly IStockDropsApiService _stockDropsApiService;
        private readonly IIdentityService _identityService;
        private readonly ILogger _logger;



        public SubscriptionService(
                                    IIdentityService identityService,
                                    ILogger<SubscriptionService> logger)
        {
            //_stockDropsApiService = stockDropsApiService;
            _identityService = identityService;
            _logger = logger;
        }

        public Task<SubscriptionLevels> GetSubscriptionLevelAsync()
        {
            return Task.FromResult(SubscriptionLevels.Gold);
            //if (_identityService.IsLoggedIn())
            //{
            //    var user = await _identityService.GetUserDefaultScopesAsync();

            //    var sub = await _stockDropsApiService.GetSubscriptionLevel(user.Id);

            //    SubscriptionLevels subscription;
            //    if (Enum.TryParse(sub?.Name, ignoreCase: true, out subscription))
            //    {
            //        return new Subscription { Name = sub.Name, AlertLimit = sub.AlertLimit, Delay = sub.Delay, SubscriptionLevel = subscription };
            //    }
            //    else
            //    {
            //        _logger.LogWarning($"Couldn't parse subscription {sub?.Name ?? "[sub null]"} {sub?.AlertLimit}");
            //    }
            //    return new Subscription { AlertLimit = 10, Delay = 2000, Name = "Free", SubscriptionLevel = SubscriptionLevels.Free };
            //    //SubscriptionLevels.Parse()
            //}
            //return new Subscription { AlertLimit = 10, Delay = 2000, Name = "Free", SubscriptionLevel = SubscriptionLevels.Free };

        }
    }
}
