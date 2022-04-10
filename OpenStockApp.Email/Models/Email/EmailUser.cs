using OpenStockApi.Core.Contracts.Models;
using System.ComponentModel.DataAnnotations;

namespace OpenStockApp.Email.Models.Email
{
    public class EmailUser : IEntity
    {
        public long Id { get; set; }
        public string? Name { get; set; }
        [Encrypted]
        public string? EmailAddress { get; set; }

        [Encrypted]
        public string? Username { get; set; }
        [Encrypted]
        public string? Password { get; set; }
        public string? Host { get; set; }

        public int SmtpPort { get; set; } = 25;
        public bool UseSSL { get; set; }

        public bool Enabled { get; set; } = false;

        public EmailUserType EmailUserType { get; set; }
    }
    public enum EmailUserType
    {
        Sender,
        Receiver
    }
}
