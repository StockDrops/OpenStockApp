using OpenStockApp.Email.Models.Email;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenStockApp.Email.Extensions
{
    public static class EmailMessageExtensions
    {
        public static Microsoft.Graph.Message ToMicrosoftMessage(this EmailMessage emailMessage)
        {
            return new Microsoft.Graph.Message
            {
                Body = new Microsoft.Graph.ItemBody
                {
                    Content = emailMessage.Body.Content,
                    ContentType = (Microsoft.Graph.BodyType?)Enum.Parse(typeof(Microsoft.Graph.BodyType), emailMessage.Body.BodyType.ToString(), true),
                },
                Subject = emailMessage.Subject,
                Sender = new Microsoft.Graph.Recipient { EmailAddress = new Microsoft.Graph.EmailAddress { Address = emailMessage.Sender.Address, Name = emailMessage.Sender.Name } },
                ToRecipients = emailMessage.Recipients.Select(x => new Microsoft.Graph.Recipient
                {
                    EmailAddress = new Microsoft.Graph.EmailAddress { Address = x.Address, Name = x.Name }
                })
            };
        }
        public static MimeKit.MimeMessage ToMimeMessage(this EmailMessage emailMessage)
        {
            return new MimeKit.MimeMessage(new List<MimeKit.InternetAddress>()
            {
                new MimeKit.MailboxAddress(emailMessage.Sender.Name, emailMessage.Sender.Address)

            }, emailMessage.Recipients.Select(x => new MimeKit.MailboxAddress(x.Name, x.Address)),
            emailMessage.Subject, new MimeKit.TextPart(emailMessage.Body.BodyType.ToString()) { Text = emailMessage.Body.Content });


        }
        public static Google.Apis.Gmail.v1.Data.Message ToGmailMessage(this EmailMessage emailMessage)
        {
            //mimekit message
            var message = emailMessage.ToMimeMessage();
            string rfcemail;
            using (var stream = new MemoryStream())
            {
                message.WriteTo(stream);
                stream.Position = 0;
                using (var reader = new StreamReader(stream))
                {
                    rfcemail = reader.ReadToEnd();
                }
            }
            //do the headers:
            //    var toHeaders = emailMessage.Recipients.Select(x => new Google.Apis.Gmail.v1.Data.MessagePartHeader { Name = "To", Value = $"\"{x.Name}\" <{x.Address}>" }).ToList();
            //var mimeType = emailMessage.Body.BodyType == BodyType.Html ? "text/html" : "text/plain";
            return new Google.Apis.Gmail.v1.Data.Message
            {
                Raw = Convert.ToBase64String(Encoding.UTF8.GetBytes(rfcemail))
            };
        }

    }
}
