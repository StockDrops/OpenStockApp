using Microsoft.Toolkit.Mvvm.ComponentModel;
using OpenStockApp.Core.Models.Users;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace OpenStockApp.Models.Users
{
    public class ObservableUser : ObservableObject
    {
        public void SetUser(User user)
        {
            UserName = user.UserPrincipalName;
            Name = user.DisplayName;
            //SubscriptionLevel = user?.Subscription;
        }

        private string? name;
        public string? Name
        {
            get => name;
            set => SetProperty(ref name, value);
        }

        private string? userName;
        public string? UserName
        {
            get => userName;
            set => SetProperty(ref userName, value);
        }

        private string? subscriptionLevel;
        public string? SubscriptionLevel
        {
            get => subscriptionLevel;
            set => SetProperty(ref subscriptionLevel, value);
        }
    }
}
