using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenStockApp.Discord.Models
{
    /// <summary>
    /// Discord webhook resource. Add additional properties to this class to deserialize them from the API response.
    /// See https://discord.com/developers/docs/resources/webhook
    /// </summary>
    public class DiscordWebhook
    {
        /// <summary>
        /// The unique webhook Id
        /// </summary>
        public string? Id { get; set; }

        /// <summary>
        /// The Guid Id aka Discord Server Id
        /// </summary>
        public string? Guild_Id { get; set; }

        /// <summary>
        /// Channel Id for the Discord channel
        /// </summary>
        public string? Channel_Id { get; set; }
    }
}
