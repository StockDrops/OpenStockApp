using MimeKit;
using OpenStockApp.Email.Models.Email;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenStockApp.Email.Contracts
{
    public interface ISmtpEmailService
    {
        EmailUser RetrieveEmailUser(int id = 0);
        MailboxAddress RetrieveMailboxAddress(int id = 0);
        void SaveEmailUser(EmailUser emailUser);
    }
}
