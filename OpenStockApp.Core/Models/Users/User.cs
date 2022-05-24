using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenStockApp.Core.Models.Users
{
    // This class contains user members to download user information from Microsoft Graph
    // https://docs.microsoft.com/graph/api/resources/user?view=graph-rest-1.0
    public class User
    {
        public string? Id { get; set; }

        public string? DisplayName { get; set; }

        public string? GivenName { get; set; }
        public string? Surname { get; set; }


        public string? Mail { get; set; }


        public string? PreferredLanguage { get; set; }

        

        public string? UserPrincipalName { get; set; }

        //public Subscription Subscription { get; set; } // TODO: add subs to the api and to the client.

        public string? Photo { get; set; } //TODO: rename this to photo url.
        
        public User() { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tokenUser"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public User(AzureTokenUser? tokenUser)
        {
            if (tokenUser == null)
            {
                throw new ArgumentNullException(nameof(tokenUser));
            }

            Id = tokenUser.oid;
            DisplayName = tokenUser.name;
            Mail = tokenUser.preferred_username;
            
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tokenUser"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public User(TokenUser? tokenUser)
        {
            if(tokenUser == null)
                throw new ArgumentNullException(nameof(tokenUser));

            if (tokenUser == null)
            {

                return;
            }
            Id = tokenUser.Oid;
            GivenName = tokenUser.Given_name;
            Surname = tokenUser.Family_name;
            if (tokenUser?.Emails?.Any() == true)
            {
                Mail = tokenUser.Emails.FirstOrDefault();
            }
            if (!string.IsNullOrEmpty(tokenUser?.Name))
            {
                if (tokenUser.Name == "unknown")
                {
                    DisplayName = $"{tokenUser.Given_name} {tokenUser.Family_name}";
                }
                else
                {
                    DisplayName = tokenUser.Name;
                }

            }
            else
            {
                var builder = new StringBuilder();
                var names = new List<string>();
                if(!string.IsNullOrEmpty(tokenUser?.Given_name))
                    names.Append(tokenUser.Given_name);
                if (!string.IsNullOrEmpty(tokenUser?.Family_name))
                    names.Add(tokenUser.Family_name);
                DisplayName = builder.AppendJoin(" ", names).ToString();
            }

        }
    }
}
