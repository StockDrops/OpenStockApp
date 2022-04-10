using OpenStockApi.Core.Models.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenStockApp.Core.Maui.Models.Configurations
{
    public class AzureClientConfiguration : AzureB2CConfiguration
    {
        public AzureAdType AzureAdType { get; set; }
        public string? ResetPasswordPolicyId { get; set; }
        public string? RedirectUrl { get; set; }
        public string? CacheName { get; set; }
        
    }
    public enum AzureAdType
    {
        B2C,
        AzureAdSingleOrganization,
        AzureAdMultipleOrganizations,
        AzureAdMultipleOrganizationsAndPersonalAccounts
    }
}
