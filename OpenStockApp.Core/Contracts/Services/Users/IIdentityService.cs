using OpenStockApp.Core.Maui.Models.Users;
using OpenStockApp.Core.Models.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenStockApp.Core.Contracts.Services.Users;

public interface IBaseIdentityService : IIdentityEvents
{
    /// <summary>
    /// Method to do any required initialization of the service.
    /// </summary>
    /// <returns></returns> Task InitializeAsync(CancellationToken cancellationToken = default)
    Task InitializeAsync(CancellationToken cancellationToken = default);
    /// <summary>
    /// Checks if the user is currently signed in.
    /// </summary>
    /// <returns>True if the user is signed in, false otherwise.</returns>
    bool IsLoggedIn();
    /// <summary>
    /// Default scopes to be used in authentication.
    /// </summary>
    IList<string> Scopes { get; }

    //#if ANDROID
    //        /// <summary>
    //        /// This function sets a default cancellation.
    //        /// </summary>
    //        /// <param name="activity"></param>
    //        /// <returns></returns>
    //        ///
    //        /// Use this to set the activity through a function.
    //        void SetActivity(Func<Android.App.Activity> getActivity);
    //        /// <summary>
    //        /// Get the activity. By default you should implement Platform.CurrentActivity. Only on Android. And on other platforms get null.
    //        /// </summary>
    //        Android.App.Activity Activity { get; }
    //#endif

    /// <summary>
    /// Use this method to control how to cancel the login.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <param name="activity"></param>
    /// <returns></returns>
    Task<LoginResultType> LoginAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Is the user authorized? 
    /// </summary>
    /// <returns></returns>
    bool IsAuthorized();
    /// <summary>
    /// Get's the user name of the user.
    /// </summary>
    /// <returns></returns>
    string? GetAccountUserName();
    string? GetUniqueId();
    /// <summary>
    /// Log out the user.
    /// </summary>
    /// <returns></returns>
    Task LogoutAsync();
    /// <summary>
    /// Get the access token linked to the default scopes.
    /// This provides the token from the cache if one is available and automatically gets a new one if it's expired.
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    ValueTask<string?> GetAccessTokenAsync(CancellationToken token = default);

    //Task<string> GetAccessTokenAsync(IEnumerable<string> scopes, CancellationToken? token = null);
    /// <summary>
    /// Request/Refresh the access token with the identity provider. This method should give a fresh access token.
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    ValueTask<bool> AcquireTokenSilentAsync(CancellationToken token = default);

    //Task<User> GetUserAsync(IEnumerable<string> scopes, CancellationToken? token = null);
    /// <summary>
    /// Get the current active user.
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    Task<User?> GetUserDefaultScopesAsync(CancellationToken token = default);
}

/// <summary>
/// This is the blueprint for any service that grabs user information from Azure only. This will not handle subscription or other SD api user information.
/// </summary>
public interface IIdentityService : IBaseIdentityService
{

}
