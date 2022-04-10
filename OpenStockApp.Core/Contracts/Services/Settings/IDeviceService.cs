using OpenStockApi.Core.Models.Users;

namespace OpenStockApp.Core.Contracts.Services.Settings
{
    public interface IDeviceService
    {
        DeviceType GetDeviceType();
        Device GetNewDevice(User user);
        Device GetNewDevice();
        string GetDeviceName();
    }
}