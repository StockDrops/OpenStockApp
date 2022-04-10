using OpenStockApp.Core.Contracts.Services.Users;
using OpenStockApp.Email.Models.Email;
using StockDrops.Core.Models.Backend.Integrations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OpenStockApp.Email.Contracts
{
    public interface IEmailService
    {
        /// <summary>
        /// Display name for the service i.e. Google or Microsoft Email
        /// </summary>
        string ServiceName { get; }
        IBaseIdentityService IdentityService { get; }
        Task<bool> SendEmailAsync(EmailMessage message, CancellationToken token);
        Task<bool> SendEmailAsync(EmailAddress destinationEmail, EmailAddress senderEmail, string subject, EmailBody emailBody, CancellationToken token);
        Task<bool> SendEmailToLoggedInUserAsync(string subject, EmailBody emailBody, CancellationToken token);
    }
}
