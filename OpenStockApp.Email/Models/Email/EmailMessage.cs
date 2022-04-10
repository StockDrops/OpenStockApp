
namespace OpenStockApp.Email.Models.Email
{
    public class EmailMessage
    {
        /// <summary>
        /// An ienumerable of email addresses of the recipients.
        /// These are To recipients. No need for BCC, or CC.
        /// </summary>
        public IEnumerable<EmailAddress> Recipients { get; set; } = new List<EmailAddress>();
        /// <summary>
        /// The sender's email.
        /// </summary>
        public EmailAddress? Sender { get; set; }
        /// <summary>
        /// This is the body of the email
        /// </summary>
        public EmailBody? Body { get; set; }
        /// <summary>
        /// The Email's subject line.
        /// </summary>
        public string? Subject { get; set; }

    }
}
