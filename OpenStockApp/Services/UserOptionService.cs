using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using OpenStockApi.Core.Models.Users;
using OpenStockApp.Core.Contracts.Services.Hubs;
using OpenStockApp.Core.Contracts.Services.Settings;
using OpenStockApp.Core.Contracts.Services.Users;

using Device = OpenStockApi.Core.Models.Users.Device;

namespace OpenStockApp.Services
{
    public class HubNotConnected : Exception
    {

    }
    public class UserOptionsService : IUserOptionsService
    {
        private readonly IIdentityService identityService;
        private readonly IMemoryCache memoryCache;
        private readonly IDeviceService deviceService;
        private readonly ILogger<UserOptionsService> logger;
        private readonly IUserOptionsHubClient userOptionsHubClient;
        public UserOptionsService(IIdentityService identityService,
            IUserOptionsHubClient userOptionsHubClient,
            IDeviceService deviceService,
            ILogger<UserOptionsService> logger,
            IMemoryCache memoryCache)
        {
            this.identityService = identityService;
            this.userOptionsHubClient = userOptionsHubClient;
            this.deviceService = deviceService;
            this.memoryCache = memoryCache;
            this.logger = logger;

            this.identityService.LoggedIn += OnLoggedIn;
            this.identityService.LoggedOut += OnLoggedOut;
        }
        ///<inheritdoc/>
        public async Task InitializeAsync(CancellationToken cancellationToken = default)
        {
            await ReloadUserOptions(cancellationToken);
        }
        private void OnLoggedOut(object? sender, EventArgs e)
        {
            //at this point the key is lost...
            //TODO: should we clean up the memory cache? I do not have the old user id after the log out.
        }

        private async void OnLoggedIn(object? sender, EventArgs e)
        {
            try
            {
                await InitializeAsync();
            }
            catch(Exception ex)
            {
                logger.LogError(ex, "Failed to initialize UserOptionsService after log in.");
            }
        }

        public UserOptions? UserOptions => GetUserOptionsFromCache();

        public ModelOptions? GetModelOptionsFromCache(long modelId)
        {
            var userOptions = GetUserOptionsFromCache();
            if(userOptions != null)
            {
                var modelOptions = userOptions.ModelOptions.Where(m => m.ModelId == modelId).FirstOrDefault();
                if(modelOptions != null)
                    return modelOptions;
                return new ModelOptions { IsEnabled = false, ModelId = modelId, NotificationAction = NotificationAction.OpenProductUrl };
            }
            return null;
        }

        //public void RegisterEvents()
        //{
        //    identityService.LoggedIn += OnLoggedIn;
        //}
        //public void OnLoggedIn(object? sender, EventArgs eventArgs)
        //{
        //    userId = identityService.GetAccountUserName();
        //}

        public UserOptions? GetUserOptionsFromCache()
        {
            if (identityService.IsLoggedIn())
            {
                var userId = identityService.GetUniqueId();
                if (userId != null && memoryCache.TryGetValue(userId, out UserOptions userOptions))
                    return userOptions;
            }
            return default;
        }

        public async Task SaveCurrentUserOptionsAsync(CancellationToken cancellationToken = default)
        {
            if(UserOptions != null)
                SetUserOptions(await userOptionsHubClient.SaveUserOptionsAsync(UserOptions, cancellationToken).ConfigureAwait(false));
            else if(userOptionsHubClient.State != Microsoft.AspNetCore.SignalR.Client.HubConnectionState.Connected)
            {
                throw new HubNotConnected();
            }
        }


        public void SetUserOptions(UserOptions? userOptions)
        {
            if(identityService.IsLoggedIn())
                memoryCache.Set(identityService.GetUniqueId(), userOptions);
        }

        private async Task ReloadUserOptions(CancellationToken cancellationToken = default)
        {
            if (identityService.IsLoggedIn())
            {
                var options = await userOptionsHubClient.GetUserOptionsAsync(cancellationToken);
                if (options is null)
                {
                    var user = await GetOrCreateUserWithDeviceAsync(cancellationToken);
                    var device = user?.Devices.FirstOrDefault(d => d.Name == DeviceInfo.Name);

                    if(user == null || device == null)
                        throw new InvalidOperationException("User or device shouldn't be null in here. This could indicate a failure to talk to our server.");

                    options = new UserOptions
                    {
                        Default = true,
                        DeviceId = device.Id,
                        UserId = user.Id
                    };
                    options = await userOptionsHubClient.SaveUserOptionsAsync(options);
                }
                SetUserOptions(options);
            }
        }
        //TODO: move some of this to the hub (server)?
        private async Task<User?> GetOrCreateUserWithDeviceAsync(CancellationToken cancellationToken = default)
        {
            var user = await userOptionsHubClient.GetUserAsync(cancellationToken);
            if(user == null)
            {
                user = new User
                {
                    AzureId = identityService.GetUniqueId()
                };
                user.Devices.Add(deviceService.GetNewDevice());
            }
            else if(user.Devices.Any())
            {
                var device = user.Devices.FirstOrDefault(d => d.Name == DeviceInfo.Name);
                if(device == null)
                {
                    user.Devices.Add(deviceService.GetNewDevice(user));
                }
            }
            else
            {
                //the use has no devices associated to it
                user.Devices.Add(deviceService.GetNewDevice(user));
            }
            return await userOptionsHubClient.SaveUserAsync(user, cancellationToken);
        }
    }
}
