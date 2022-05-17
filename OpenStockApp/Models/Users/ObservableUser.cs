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
            UserName = user.UserPrincipalName ?? "";
            Name = user.DisplayName ?? "";
            UserProfileSource = user.Photo ?? "";
            //SubscriptionLevel = user?.Subscription;
        }

        private string name = string.Empty;
        public string Name
        {
            get => name;
            set => SetProperty(ref name, value);
        }

        private string userName = string.Empty;
        public string UserName
        {
            get => userName;
            set => SetProperty(ref userName, value);
        }

        private string subscriptionLevel = string.Empty;
        public string SubscriptionLevel
        {
            get => subscriptionLevel;
            set => SetProperty(ref subscriptionLevel, value);
        }
        private string userProfileSource = string.Empty;
        public string UserProfileSource
        {
            get => userProfileSource;
            set => SetProperty(ref userProfileSource, value);
        }
    }
}
