using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenStockApp.Core.Services.Api
{
    internal class DefaultRetryPolicy : IRetryPolicy
    {
        
        public TimeSpan? NextRetryDelay(RetryContext retryContext)
        {
            
            throw new NotImplementedException();
        }
    }
}
