using Microsoft.Toolkit.Mvvm.Input;
using OpenStockApi.Core.Models.Users;
using OpenStockApp.Core.Contracts.Services.Hubs;
using OpenStockApp.SignalR.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenStockApp.ViewModels.AlertSettings
{
    public class UserOptionsViewModel : BindableBaseViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        /// 
        public AsyncRelayCommand LoadUserOptions { get; set; }
        private IUserOptionsHubClient userOptionsHubClient;


        static BindableProperty UserOptionsProperty =
            BindableProperty.Create(nameof(UserOptions), typeof(UserOptions), typeof(UserOptionsViewModel),  null);

        public UserOptions? UserOptions
        {
            get => (UserOptions)GetValue(UserOptionsProperty);
            set => SetValue(UserOptionsProperty, value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userOptionsHub"></param>
        public UserOptionsViewModel(IUserOptionsHubClient userOptionsHub)
        {
            userOptionsHubClient = userOptionsHub;

            LoadUserOptions = new AsyncRelayCommand(OnLoadUserOptions);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task OnLoadUserOptions(CancellationToken cancellationToken = default)
        {
            var userOptions = await userOptionsHubClient.GetUserOptionsAsync(cancellationToken);
            UserOptions = userOptions; 
        }

    }
}
