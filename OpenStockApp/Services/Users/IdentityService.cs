
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;


using OpenStockApp.Core.Maui.Models.Configurations;
using OpenStockApp.Core.Maui.Models.Users;

using OpenStockApp.Core.Models.Users;
using OpenStockApp.Core.Contracts.Services.Users;
using OpenStockApp.Core.Maui.Helpers;


#if ANDROID
using Android.App;
#elif WINDOWS
using Microsoft.Identity.Client.Extensions.Msal;
#endif

namespace OpenStockApp.Services.Users
{
    public class IdentityService : IIdentityService
    {
        /*
        For more information about using Identity, see
        https://github.com/microsoft/WindowsTemplateStudio/blob/release/docs/WPF/services/identity.md

        Read more about Microsoft Identity Client here
        https://github.com/AzureAD/microsoft-authentication-library-for-dotnet/wiki
        https://docs.microsoft.com/azure/active-directory/develop/v2-overview

        TODO WTS: Please create a ClientID following these steps and update the appsettings.json IdentityClientId.
        https://docs.microsoft.com/azure/active-directory/develop/quickstart-register-app

        The provided clientID requests permissions on user.read, this might be blocked in environments that require admin consent.
        For more info about admin consent please see https://docs.microsoft.com/azure/active-directory/develop/application-consent-experience
        For more info creating protected APIs, see https://docs.microsoft.com/azure/active-directory/develop/scenario-protected-web-api-overview
        For more info on desktop apps that call protected APIs, see https://docs.microsoft.com/azure/active-directory/develop/scenario-desktop-overview
        */

        /// /// <summary>
        /// Client defined by MSAL.
        /// </summary>
        protected readonly IPublicClientApplication client;
        public IList<string> Scopes => azureClientConfiguration.Scopes?.Split(" ", options: StringSplitOptions.RemoveEmptyEntries) ?? throw new ArgumentNullException("Scopes cannot be null");
        private string? AccountIdentifierUsedInInteractive { get; set; }

        private bool _integratedAuthAvailable = false;


        private AuthenticationResult? _authenticationResult;
        private string PasswordResetAuthority
        {
            get
            {
                switch (azureClientConfiguration.AzureAdType)
                {
                    case AzureAdType.B2C:
                        return $"{azureClientConfiguration.Instance}/tfp/{azureClientConfiguration.Domain}/{azureClientConfiguration.ResetPasswordPolicyId}";
                    default:
                        return "";
                }

            }
        }


        private string DefaultAuthority
        {
            get
            {
                switch (azureClientConfiguration.AzureAdType)
                {
                    case AzureAdType.B2C:
                        return $"{azureClientConfiguration.Instance}/tfp/{azureClientConfiguration.Domain}/{azureClientConfiguration.SignUpSignInPolicyId}";
                    default:
                        return "";  //TODO: make this compatible with non b2c.
                }
            }
        }

        private string? UsedAuthority { get; set; }

        private double _timeOutLogin = 10; //in minutes


        protected readonly ILogger<IdentityService> logger;

        public event EventHandler? LoggedIn;
        public event EventHandler? LoggedOut;

        private readonly AzureClientConfiguration azureClientConfiguration;
        /// <summary>
        /// A constructor
        /// </summary>
        /// <param name="options"></param>
        /// <param name="logger"></param>
        public IdentityService(IOptions<AzureClientConfiguration> options, ILogger<IdentityService> logger)
        {
            this.logger = logger;
            azureClientConfiguration = options.Value;

            if (azureClientConfiguration.RedirectUrl == null)
                throw new ArgumentNullException(nameof(options.Value.RedirectUrl));
            if (azureClientConfiguration.ClientId == null)
                throw new ArgumentNullException(nameof(options.Value.ClientId));

#if ANDROID || IOS
            azureClientConfiguration.RedirectUrl = $"msal{azureClientConfiguration.ClientId}://auth";
#endif


            switch (azureClientConfiguration.AzureAdType)
            {
                case AzureAdType.B2C:

                    if (azureClientConfiguration.Instance == null)
                        throw new ArgumentNullException(nameof(options.Value.Instance));
                    if (azureClientConfiguration.Domain == null)
                        throw new ArgumentNullException(nameof(options.Value.Domain));
                    if (azureClientConfiguration.SignUpSignInPolicyId == null)
                        throw new ArgumentNullException(nameof(options.Value.SignUpSignInPolicyId));


                    var b2cAuthority = $"{azureClientConfiguration.Instance}/tfp/{azureClientConfiguration.Domain}/{azureClientConfiguration.SignUpSignInPolicyId}";
                    var passwordResetAuthority = $"{azureClientConfiguration.Instance}/tfp/{azureClientConfiguration.Domain}/{azureClientConfiguration.ResetPasswordPolicyId}";
                    client = InitializeWithAadB2C(azureClientConfiguration.ClientId, b2cAuthority, passwordResetAuthority, azureClientConfiguration.RedirectUrl);
                    break;

                case AzureAdType.AzureAdSingleOrganization:
                    if (azureClientConfiguration.TenantId == null)
                        throw new ArgumentNullException(nameof(azureClientConfiguration.TenantId));
                    client = InitializeWithAadSingleOrg(azureClientConfiguration.ClientId, azureClientConfiguration.TenantId, redirectUri: azureClientConfiguration.RedirectUrl);
                    break;

                case AzureAdType.AzureAdMultipleOrganizations:
                    client = InitializeWithAadMultipleOrgs(azureClientConfiguration.ClientId, redirectUri: azureClientConfiguration.RedirectUrl);
                    break;
                case AzureAdType.AzureAdMultipleOrganizationsAndPersonalAccounts:
                    client = InitializeWithAadAndPersonalMsAccounts(azureClientConfiguration.ClientId, redirectUri: azureClientConfiguration.RedirectUrl);
                    break;
                default:
                    throw new ArgumentException("Invalid Azure Ad Type");
            }
            //ConfigureCache().Wait(1000);
        }

        public bool IsLoggedIn()
        {
            return _authenticationResult != null;
        }

        public async Task<LoginResultType> LoginAsync(CancellationToken token = default)
        {
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.CancelAfter(TimeSpan.FromMinutes(_timeOutLogin));
            CancellationToken cancellationToken = token;


            var embeddedView = true;
#if WINDOWS
            if (Environment.OSVersion.Version < new Version("10.0"))
                embeddedView = false;
#elif ANDROID
            embeddedView = false;
            
#else
            
            embeddedView = false;
#endif
            var options = new SystemWebViewOptions()
            {
                OpenBrowserAsync = SystemWebViewOptions.OpenWithChromeEdgeBrowserAsync,
                iOSHidePrivacyPrompt = true,
            };

            if (!ConnectivityHelpers.IsInternetAvailable())
                return LoginResultType.NoNetworkAvailable;

            try
            {

                _authenticationResult = await client.AcquireTokenInteractive(Scopes)
                                            .WithUseEmbeddedWebView(embeddedView)
                                            .WithSystemWebViewOptions(options)
                                            
#if ANDROID

                                            
                                                                                                                   .WithParentActivityOrWindow(Platform.CurrentActivity)
#endif
                                                            .ExecuteAsync(cancellationToken);

                UpdateLatestIdentifierUsedInteractively();

                LoggedIn?.Invoke(this, EventArgs.Empty);
                return LoginResultType.Success;
            }
            catch (MsalClientException ex)
            {
                System.Diagnostics.Debug.WriteLine($"{ex}");
                if (ex.ErrorCode == "authentication_canceled")
                    return LoginResultType.CancelledByUser;
#if DEBUG
                if(ex.ErrorCode == "missing_entitlements")
                {
                    return LoginResultType.Success;
                }
#endif
                if (embeddedView)
                    embeddedView = false;
                logger?.LogError(ex, "");
                return LoginResultType.UnknownError;
            }
            catch (MsalException ex)
            {
                if (ex.Message.Contains("AADB2C90118"))
                {
                    try
                    {

                        //var accounts = await _client.GetAccountsAsync();
                        _authenticationResult = await client.AcquireTokenInteractive(Scopes)
                            .WithUseEmbeddedWebView(embeddedView)
                            //.WithAccount(accounts.FirstOrDefault())
                            .WithPrompt(Prompt.SelectAccount)
                            .WithB2CAuthority(PasswordResetAuthority)
                            //#if ANDROID
                            //                                                       .WithParentActivityOrWindow(Activity)
                            //#endif
                            .ExecuteAsync();
                        UpdateLatestIdentifierUsedInteractively();
                        UsedAuthority = PasswordResetAuthority;
                        LoggedIn?.Invoke(this, EventArgs.Empty);
                        return LoginResultType.Success;

                    }
                    catch (Exception e)
                    {
                        logger?.LogError(e, "");
                        System.Diagnostics.Debug.WriteLine(ex);
                        return LoginResultType.UnknownError;
                    }

                }
                System.Diagnostics.Debug.WriteLine(ex);
                logger?.LogError(ex, "");
                return LoginResultType.UnknownError;
            }
            catch (OperationCanceledException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                return LoginResultType.TimedOut;
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "");
                System.Diagnostics.Debug.WriteLine(ex);
                return LoginResultType.UnknownError;
            }
        }

        public bool IsAuthorized()
        {
            // TODO WTS: You can also add extra authorization checks here.
            // i.e.: Checks permisions of _authenticationResult.Account.Username in a database.
            return true;
        }

        public string? GetAccountUserName()
        {
            return _authenticationResult?.Account?.Username;
        }
        public string? GetUniqueId()
        {
            return _authenticationResult?.UniqueId;
        }

        public async Task LogoutAsync()
        {
            try
            {
                var accounts = await client.GetAccountsAsync();
                var account = accounts.FirstOrDefault();
                if (account != null)
                {
                    await client.RemoveAsync(account);
                }

                _authenticationResult = null;
                LoggedOut?.Invoke(this, EventArgs.Empty);
            }
            catch (MsalException)
            {
                // TODO WTS: LogoutAsync can fail please handle exceptions as appropriate to your scenario
                // For more info on MsalExceptions see
                // https://github.com/AzureAD/microsoft-authentication-library-for-dotnet/wiki/exceptions
            }
        }

        ///<inheritdoc/>
        public async ValueTask<string?> GetAccessTokenAsync(CancellationToken token = default) => await GetAccessTokenAsync(Scopes, token);

        // All sensitive data in your app should be retrieven using access tokens.
        // This method provides you with an access token to secure calls to the Microsoft Graph or other protected API.
        // For more info on protecting web api with tokens see https://docs.microsoft.com/azure/active-directory/develop/scenario-protected-web-api-overview
        public async ValueTask<string?> GetAccessTokenAsync(IEnumerable<string> scopes, CancellationToken token = default)
        {
            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.CancelAfter(TimeSpan.FromMinutes(_timeOutLogin));
            var retryAttempt = 0;
            var embeddedView = true;
        retry:
            var acquireTokenSuccess = await AcquireTokenSilentAsync(scopes, token);
            if (acquireTokenSuccess)
            {
                return _authenticationResult?.AccessToken ?? throw new InvalidOperationException("AuthenticationResult shouldn't be null here after a succesful token acquisition.");
            }
            else if (!ConnectivityHelpers.IsInternetAvailable())
            {
                if (retryAttempt < 20)
                {
                    retryAttempt++;
                    await Task.Delay((int)(1000 * Math.Pow(2, retryAttempt)));
                    goto retry;
                }
                return null;
            }
            else
            {
                try
                {
                    // Interactive authentication is required


                    //var accounts = await _client.GetAccountsAsync();
                    _authenticationResult = await client.AcquireTokenInteractive(scopes)
                                                         .WithUseEmbeddedWebView(embeddedView)
                                                         //#if ANDROID
                                                         //                                                       .WithParentActivityOrWindow(Activity)
                                                         //#endif
                                                         //.WithAccount(accounts.FirstOrDefault())
                                                         .ExecuteAsync(token);
                    UpdateLatestIdentifierUsedInteractively();
                    return _authenticationResult.AccessToken;
                }
                catch (MsalException)
                {
                    // AcquireTokenSilent and AcquireTokenInteractive failed, the session will be closed.
                    _authenticationResult = null;
                    LoggedOut?.Invoke(this, EventArgs.Empty);
                    return string.Empty;
                }
                catch (Exception e)
                {
                    if (embeddedView)
                    {
                        embeddedView = false;
                        goto retry;
                    }

                    logger?.LogError(e, "");
                    //_authenticationResult = null;
                    //LoggedOut?.Invoke(this, EventArgs.Empty);
                    return string.Empty;
                }
            }
        }
        public async Task<User?> GetUserDefaultScopesAsync(CancellationToken token = default) => await GetUserAsync(Scopes, token);
        public async Task<User?> GetUserAsync(IEnumerable<string> scopes, CancellationToken token = default)
        {
            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.CancelAfter(TimeSpan.FromMinutes(_timeOutLogin));

            var embeddedView = true;
        retry:
            var acquireTokenSuccess = await AcquireTokenSilentAsync(scopes, token);
            if (acquireTokenSuccess)
            {
                var user = ParseUser(_authenticationResult?.IdToken ?? throw new InvalidOperationException("AuthenticationResult shouldn't be null here after a succesful token acquisition."));
                //let's call our api to get the subscription info
                //user.Subscription = await _getSubscriptionAsync(_authenticationResult.AccessToken, user.Id);

                return user;
            }
            else
            {
                try
                {
                    // Interactive authentication is required


                    var accounts = await client.GetAccountsAsync();
                    var account = accounts.FirstOrDefault();
                    _authenticationResult = await client.AcquireTokenInteractive(scopes)
                                                           .WithUseEmbeddedWebView(embeddedView)
                                                         .WithAccount(account)
                                                         .ExecuteAsync(token);
                    UpdateLatestIdentifierUsedInteractively();
                    var user = ParseUser(_authenticationResult.IdToken);

                    //let's call our api to get the subscription info
                    //user.Subscription = await _getSubscriptionAsync(_authenticationResult.AccessToken, user.Id);
                    return user;
                }
                catch (MsalException)
                {
                    // AcquireTokenSilent and AcquireTokenInteractive failed, the session will be closed.
                    if (embeddedView)
                    {
                        embeddedView = false;
                        goto retry;
                    }
                    _authenticationResult = null;
                    LoggedOut?.Invoke(this, EventArgs.Empty);
                    return null;
                }
                catch (Exception e)
                {
                    if (embeddedView)
                    {
                        embeddedView = false;
                        goto retry;
                    }
                    _authenticationResult = null;
                    logger.LogError(e, "Error in GetUserAsync");
                    LoggedOut?.Invoke(this, EventArgs.Empty);
                    return null;
                }
            }
        }

        public async ValueTask<bool> AcquireTokenSilentAsync(CancellationToken token = default) => await AcquireTokenSilentAsync(Scopes.ToArray(), token);
        //public async Task<Subscription> GetSubscriptionAsync()
        //{
        //    var user = await GetUserDefaultScopesAsync();
        //    return await _getSubscriptionAsync(_authenticationResult.AccessToken, user.Id);
        //}

        private void UpdateLatestIdentifierUsedInteractively()
        {
            AccountIdentifierUsedInInteractive = _authenticationResult?.Account.HomeAccountId?.Identifier;
            //try
            //{
            //    //we save into the json with the settings to be able to reuse it later.
            //    _identityCacheService.SaveAccountId(_authenticationResult.Account.HomeAccountId);
            //}
            //catch (Exception e)
            //{
            //    logger?.LogError(e, "");
            //}

        }

        private async Task<bool> AcquireTokenSilentAsync(IEnumerable<string> scopes, CancellationToken cancellationToken = default)
        {
            //var retryPolicy = Policy
            //                    .Handle<HttpRequestException>()
            //                    .Or<SocketException>()
            //                    .WaitAndRetryAsync(20, retryAttempt =>
            //                        TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

            if (!ConnectivityHelpers.IsInternetAvailable())
            {
                logger?.LogError("LostNetworkDuringToken");
                return false;
            }

            try
            {
                //uses Polly to retry it multiple times in case of error.
                // var result = await retryPolicy.ExecuteAsync(async () =>
                //{
                IAccount? account = null;
                if (AccountIdentifierUsedInInteractive != null)
                {
                    account = await client.GetAccountAsync(AccountIdentifierUsedInInteractive);
                }
                else
                {
                    account = (await client.GetAccountsAsync()).FirstOrDefault();
                }
                //var accounts = await _client.GetAccountsAsync(); //this line sometimes gives No such host exception
                _authenticationResult = await client.AcquireTokenSilent(scopes, account).ExecuteAsync().ConfigureAwait(false);
                return true;
                //});
                //return result;

            }
            catch (MsalUiRequiredException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                if (_integratedAuthAvailable)
                {
                    try
                    {
                        _authenticationResult = await client.AcquireTokenByIntegratedWindowsAuth(Scopes)
                                                             .ExecuteAsync();
                        return true;
                    }
                    catch (MsalUiRequiredException)
                    {
                        logger?.LogError("MsalUiRequiredException");
                        // Interactive authentication is required
                        return false;
                    }
                }
                else
                {
                    logger?.LogError("IntegratedAuthNotAvailable");
                    // Interactive authentication is required
                    return false;
                }
            }
            catch (MsalException)
            {
                // TODO WTS: Silentauth failed, please handle this exception as appropriate to your scenario
                // For more info on MsalExceptions see
                // https://github.com/AzureAD/microsoft-authentication-library-for-dotnet/wiki/exceptions
                logger?.LogError("MsalException");
                return false;
            }
            catch (Exception e)
            {
                logger?.LogError(e, "");
                return false;
            }
        }

        /// <summary>
        /// Configures the cache to use in desktop apps. It does nothing on mobile platforms.
        /// </summary>
        /// <returns></returns>
        private async Task ConfigureCache()
        {
#if WINDOWS || MACCATALYST
            try
            {
                var folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "StockDrops\\Cache");
                var storageProperties = new StorageCreationPropertiesBuilder(
                             ".msal",
                             folder)
                         .WithMacKeyChain("stockdrops_msal_service", "stockdrops_msal_account")
                         .Build();
                var cacheHelper = await MsalCacheHelper.CreateAsync(storageProperties);

                cacheHelper.RegisterCache(client.UserTokenCache);
            }
            catch (Exception e)
            {
                logger.LogError(e, "");
            }

#endif

        }
#if ANDROID
        private Activity MainActivity => Platform.CurrentActivity;
#endif
        private IPublicClientApplication InitializeWithAadAndPersonalMsAccounts(string clientId, string? redirectUri = null)
        {

            return PublicClientApplicationBuilder.Create(clientId)
                                                    .WithAuthority(AadAuthorityAudience.AzureAdAndPersonalMicrosoftAccount)
                                                    .WithRedirectUri(redirectUri)
                                                    .Build();
        }
        private IPublicClientApplication InitializeWithAadB2C(string clientId, string authority, string? passwordResetAuthority = null, string? redirectUri = null)
        {
            _integratedAuthAvailable = false;
            UsedAuthority = authority;

            var clientBuilder = PublicClientApplicationBuilder.Create(clientId)
                                                    .WithB2CAuthority(authority);
#if ANDROID
                                                    clientBuilder.WithParentActivityOrWindow(() => MainActivity);
#elif IOS || MACCATALYST
                                                    clientBuilder.WithIosKeychainSecurityGroup("com.stockdrops.openstockapp");
#endif
           return clientBuilder.WithRedirectUri(redirectUri)
                                                    .Build();
        }
        private IPublicClientApplication InitializeWithAadMultipleOrgs(string clientId, bool integratedAuth = false, string? redirectUri = null)
        {
            _integratedAuthAvailable = integratedAuth;

            return PublicClientApplicationBuilder.Create(clientId)
                                                    .WithAuthority(AadAuthorityAudience.AzureAdMultipleOrgs)
                                                    .WithRedirectUri(redirectUri)
                                                    .Build();
        }

        private IPublicClientApplication InitializeWithAadSingleOrg(string clientId, string tenant, bool integratedAuth = false, string? redirectUri = null)
        {
            _integratedAuthAvailable = integratedAuth;
            return PublicClientApplicationBuilder.Create(clientId)
                                                    .WithAuthority(AzureCloudInstance.AzurePublic, tenant)
                                                    .WithRedirectUri(redirectUri)
                                                    .Build();

            //ConfigureCache();
        }

        private User ParseUser(string idToken)
        {
            idToken = idToken.Split('.')[1];
            idToken = Base64UrlDecode(idToken);
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };
            if (azureClientConfiguration.AzureAdType == AzureAdType.B2C)
                return new User(JsonSerializer.Deserialize<TokenUser>(idToken, options));
            return new User(JsonSerializer.Deserialize<AzureTokenUser>(idToken, options));
        }
        private string Base64UrlDecode(string s)
        {
            s = s.Replace('-', '+').Replace('_', '/');
            s = s.PadRight(s.Length + (4 - s.Length % 4) % 4, '=');
            var byteArray = Convert.FromBase64String(s);
            var decoded = Encoding.UTF8.GetString(byteArray, 0, byteArray.Count());
            return decoded;
        }
        public async Task InitializeAsync(CancellationToken cancellationToken = default)
        {
            //configure the cache:
            await ConfigureCache();
            //try to get the token silently:
            await AcquireTokenSilentAsync(cancellationToken);
        }
    }
}