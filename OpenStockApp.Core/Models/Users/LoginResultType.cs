namespace OpenStockApp.Core.Maui.Models.Users
{
    public enum LoginResultType
    {
        Success,
        Unauthorized,
        CancelledByUser,
        NoNetworkAvailable,
        TimedOut,
        UnknownError

    }
}
