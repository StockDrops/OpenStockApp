using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenStockApi.Core.Models.Products;
using OpenStockApi.Core.Models.Regions;
using OpenStockApi.Core.Models.Searches;
using OpenStockApi.Core.Models.Users;
using OpenStockApp.Core.Contracts.Services.Hubs;
using OpenStockApp.Core.Contracts.Services.Settings;
using OpenStockApp.Core.Contracts.Services.Users;

namespace OpenStockApp.SignalR.Services
{
    public class UserOptionsHubClient : BaseHubClient, IUserOptionsHubClient
    {

        private IDeviceService deviceService;
        public UserOptionsHubClient(IIdentityService identityService, 
            IOptions<BaseHubConfiguration> options,
            IDeviceService deviceService,
            ILogger<BaseHubClient> logger) : base(identityService, options, logger)
        {

            this.deviceService = deviceService;
        }


        protected override async void OnLoggedIn(object? sender, EventArgs eventArgs)
        {
            base.OnLoggedIn(sender, eventArgs);

            //try
            //{
            //    var userOptions = await GetUserOptionsAsync();
            //    if(userOptions != null)
            //        userOptionsService.SetUserOptions(userOptions);
            //    else
            //    {
            //        var user = await GetUserAsync();
            //        Device device = null;
            //        if(user != null && !user.Devices.Any(d => d.Name == deviceService.GetDeviceName()))
            //        {
            //            device = deviceService.GetNewDevice(user);
            //        }
            //        else if(user != null && user.Devices.Any(d => d.Name == deviceService.GetDeviceName()))
            //        {
            //            device = user.Devices.First(d => d.Name == deviceService.GetDeviceName());
            //        }
            //        else if(user == null)
            //        {
            //            //no user registered. Let's add ourselves.
            //            user = new User
            //            {
            //                AzureId = identityService.GetUniqueId(),

            //            };
            //            user.Devices.Add(deviceService.GetNewDevice());

            //        }


            //    }
            //}
            //catch { }
        }

        public async Task<User?> GetUserAsync(CancellationToken cancellationToken = default)
        {
            //var userId = identityService.GetUniqueId();
            return await connection.InvokeAsync<User?>("GetUser", cancellationToken);
        }

        public async Task<User?> SaveUserAsync(User user, CancellationToken cancellationToken = default)
        {
            return await connection.InvokeAsync<User?>("SaveUser", user, cancellationToken);
        }

        public async Task<Device?> GetDeviceAsync(string deviceName, CancellationToken cancellationToken = default)
        {
            return await connection.InvokeAsync<Device?>("GetDevice", new DeviceRequestOptions { DeviceName = deviceName}, cancellationToken: cancellationToken);
        }

        public async Task<IEnumerable<Model>> GetModels(Product product, CancellationToken cancellationToken = default)
        {
            try
            {
                if(product == null)
                    return Enumerable.Empty<Model>();
                return await connection.InvokeAsync<IEnumerable<Model>>("GetModels", product, cancellationToken);
            }
            catch(Exception ex)
            {
                logger.LogError(ex, "Failed to load models");
            }
            return new List<Model>();
        }

        public async Task<IEnumerable<Product>> GetProducts(CancellationToken cancellationToken = default)
        {
            try
            {
                return await connection.InvokeAsync<IEnumerable<Product>>("GetProducts", cancellationToken);
            }
            catch(Exception ex)
            {
                logger.LogError(ex, "Failed to load products");
            }
            return new List<Product>();
        }

        public async Task<UserOptions?> SaveUserOptionsAsync(OpenStockApi.Core.Models.Users.UserOptions userOptions, CancellationToken cancellationToken = default)
        {
            try
            {
                return await connection.InvokeAsync<UserOptions?>("SaveUserOptions", userOptions, cancellationToken);
            }
            catch(Exception ex)
            {
                logger.LogError(ex, "Failed to save user options");
            }
            return default;
        }
        public async Task<UserOptions?> GetUserOptionsAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var userOptions = await connection.InvokeAsync<OpenStockApi.Core.Models.Users.UserOptions>("GetUserOptions", cancellationToken);
                //if(userOptions == null)
                //{
                //    //we must recreate some for the user.

                //}


                //SetUserOptions(userOptions);
                return userOptions;
                
            }
            catch(Exception ex)
            {
                logger.LogError(ex, "Failed to load user options");
            }
            return null;
        }

        public async Task<ModelOptions?> SaveModelOptions(ModelOptions modelOptions, CancellationToken cancellationToken = default)
        {
            try
            {
                return await connection.InvokeAsync<ModelOptions?>("SaveModelOptions", modelOptions, cancellationToken);
            }
            catch(Exception ex)
            {
                logger.LogError(ex, "Failed to save model options");
            }
            return default;
        }
        public async Task SetUserOptions(UserOptions? userOptions)
        {

        }
        protected override void RegisterHubFunctions()
        {
            connection.On<UserOptions?>("SetUserOptions", SetUserOptions);
        }

        

        public async Task<IEnumerable<Country>> GetCountries(CancellationToken cancellationToken = default)
        {
            try
            {
                return await connection.InvokeAsync<IEnumerable<Country>>("GetCountries", cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to load latest countries");
            }
            return new List<Country>();
        }

        public async Task<IEnumerable<Retailer>> GetRetailersByCountry(Country country, CancellationToken cancellationToken = default)
        {
            try
            {
                if (country == null)
                    return new List<Retailer>();
                return await connection.InvokeAsync<IEnumerable<Retailer>>("GetRetailersByCountry", country, cancellationToken);
            }
            catch(Exception ex)
            {
                logger.LogError(ex, "Failed to load retailer for {country}", country.Name);
            }
            return new List<Retailer>();
        }
    }
}
