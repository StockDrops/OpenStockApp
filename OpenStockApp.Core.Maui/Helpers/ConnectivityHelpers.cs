using System.Net.NetworkInformation;

namespace OpenStockApp.Core.Maui.Helpers
{
    public static class ConnectivityHelpers
    {
        /// <summary>
        /// Use this method to know if you have internet available. It is platform agnostic.
        /// For Android/IOS, it will use connectivity from MAUI essentials.
        /// We will try to use the same on Windows with a try/catch block in case it's not available on WPF if the library is used on WPF
        /// but will work in UWP.
        /// </summary>
        /// <returns></returns>
        public static bool IsInternetAvailable()
        {
            try
            {
                if (
                    Connectivity.NetworkAccess == NetworkAccess.Internet
#if WINDOWS
                    || NetworkInterface.GetIsNetworkAvailable()
#endif
                    )
                {
                    return true;
                }
                return false;
            }
            catch
            {
                try
                {
                    //then we try the regular
                    return NetworkInterface.GetIsNetworkAvailable();
                }
                catch
                {
                    return false;
                }
            }
        }
    }
}
