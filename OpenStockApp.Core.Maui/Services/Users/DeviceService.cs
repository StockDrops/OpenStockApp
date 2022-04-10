using OpenStockApi.Core.Models.Users;
using OpenStockApp.Core.Contracts.Services.Settings;

namespace OpenStockApp.Core.Maui.Services.Users
{
    public class DeviceService : IDeviceService
    {
        public OpenStockApi.Core.Models.Users.Device GetNewDevice(User user)
        {
            var device = new OpenStockApi.Core.Models.Users.Device()
            {
                DeviceType = GetDeviceType(),
                Id = 0,
                Name = DeviceInfo.Name,
                UserId = user.Id
            };
            return device;
        }
        public OpenStockApi.Core.Models.Users.Device GetNewDevice()
        {
            return new OpenStockApi.Core.Models.Users.Device()
            {
                DeviceType = GetDeviceType(),
                Id = 0,
                Name = DeviceInfo.Name,
                UserId = 0
            };
        }

        public string GetDeviceName()
        {
            return DeviceInfo.Name;
        }

        public OpenStockApi.Core.Models.Users.DeviceType GetDeviceType()
        {
#if WINDOWS
            return OpenStockApi.Core.Models.Users.DeviceType.WindowsPC;
#elif MACCATALYST
            return OpenStockApi.Core.Models.Users.DeviceType.MacOS;
#elif ANDROID
            return OpenStockApi.Core.Models.Users.DeviceType.Android;
#elif IOS
            return OpenStockApi.Core.Models.Users.DeviceType.iOS;
#else
            throw new InvalidOperationException("Unknown platform");
#endif
        }
    }
}
