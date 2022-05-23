#if WINDOWS
using Microsoft.Win32;
#endif
namespace OpenStockApp.Services
{
    public static class BackgroundServicesContainer
    {
        private static List<Task> backgroundTasks = new();
        private static CancellationTokenSource tokenSource = new();
        private static ApplicationHostService? applicationHostService;
        public static void StartBackgroundTasks()
        {
#if WINDOWS
            
#endif
        }
        /// <summary>
        /// This will block until all has started.
        /// </summary>
        public static void StartApp()
        {
            //This operation is crucial for the app to function. If they don't the app should crash.
            applicationHostService = ServiceLocator.GetRequiredService<ApplicationHostService>();

            Task.Run(async () => await applicationHostService.StartAsync(tokenSource.Token)).Wait();
        }

        public static void StopApp()
        {
            if(applicationHostService != null)
            {
                Task.Run(async () => await applicationHostService.StopAsync()).Wait(1000);
            }
            //_ = applicationHostService?.StopAsync();
        }
#if WINDOWS
        public static void OnPowerStateChanged(object sender, PowerModeChangedEventArgs e)
        {
            if(e.Mode == PowerModes.Resume)
            {
                StartApp();
            }
            else if(e.Mode == PowerModes.Suspend)
            {
                StopApp();
            }
        }
#endif
        public static void Resume()
        {

        }
    }
}
