using OpenStockApp.Core.Contracts.Services;
using OpenStockApp.SignalR.Services;
using Microsoft.Maui.LifecycleEvents;
using OpenStockApp.Services;
using OpenStockApp.Core.Contracts.Services.Users;
using OpenStockApp.Core.Maui.Services.Users;
using OpenStockApp.Core.Maui.Models.Configurations;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using OpenStockApp.ViewModels.Settings;
using OpenStockApp.Models;
using OpenStockApp.Ef.Models;
using OpenStockApp.Core.Contracts.Services.Settings;
using OpenStockApp.Core.Maui.Services.Settings;
using OpenStockApp.Email.Models.Context;
using OpenStockApp.Discord.Contracts.Services;
using OpenStockApp.Discord.Services;
using OpenStockApp.Core.Services;
using OpenStockApp.Extensions;
using OpenStockApp.Core.Contracts.Services.Api;
using OpenStockApp.Core.Services.Api;
using OpenStockApp.LegacyApi.Services;
using System.Security.Cryptography.X509Certificates;
using System.Net;
using OpenStockApp.Core.Maui.Helpers;
using OpenStockApp.ViewModels.AlertSettings;
using OpenStockApp.Pages.Alerts;
using OpenStockApp.Pages.Settings;
using OpenStockApp.ViewModels;
using CommunityToolkit.Maui;
using OpenStockApp.Core.Contracts.Services.Hubs;
using OpenStockApp.Core.Maui.Services.Notifications;
using OpenStockApp.Core.Maui.Contracts.Services;
using Microsoft.Extensions.Logging;
using OpenStockApp.Services.Users;
using OpenStockApp.Maui.Platforms.Android;



#if WINDOWS
using Microsoft.Toolkit.Uwp.Notifications;
using Windows.ApplicationModel.Activation;
using OpenStockApp.Core.Maui.Platforms.Windows;
#endif
namespace OpenStockApp;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiCommunityToolkit()
            
            .UseMauiApp<App>();
			//.ConfigureFonts(fonts =>
			//{
			//	fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
			//});
        builder.ConfigureLifecycleEvents(lifecycle =>
        {
#if WINDOWS
            lifecycle.AddWindows(configure =>
            {
                configure.OnLaunching((app, args) =>
                {
                    BackgroundServicesContainer.StartApp();
                });
                configure.OnLaunched((window, args) =>
                {
                    ToastNotificationManagerCompat.OnActivated += WindowsNotificationService.OnActivated;
                });
               configure.OnClosed((app, args) => BackgroundServicesContainer.StopApp());
            });
#elif ANDROID
                lifecycle.AddAndroid(configure =>
                {
                    configure.OnApplicationCreating(activity =>
                   {
                       Platform.Init(activity);
                       BackgroundServicesContainer.StartApp();
                       
                   });
                });
#endif
        });
        //configuration:
        var assembly = Assembly.GetExecutingAssembly();
        // Look up the resource names and find the one that ends with settings.json
        // Your resource names will generally be prefixed with the assembly's default namespace
        // so you can short circuit this with the known full name if you wish

        var ConfigFile = assembly.GetManifestResourceNames()
            ?.FirstOrDefault(r => r.EndsWith("settings.json", StringComparison.OrdinalIgnoreCase));
        if(ConfigFile == null)
            throw new ArgumentNullException(nameof(ConfigFile));
        //Load configuration file from the stream gotten above.
        using var stream = assembly.GetManifestResourceStream(ConfigFile);
        builder.Configuration.AddJsonStream(stream);

        builder.Services.AddLogging(loggingBuilder =>
        {
            
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "StockDrops\\logs\\applog.txt");
            loggingBuilder.AddFile(path, fileLoggerOptions =>
            {
                fileLoggerOptions.FileSizeLimitBytes = 5000000;
                fileLoggerOptions.MaxRollingFiles = 4;
                fileLoggerOptions.MinLevel = LogLevel.Information;
                fileLoggerOptions.Append = false;
                fileLoggerOptions.UseUtcTimestamp = true;
            });
            loggingBuilder.AddFilter("Microsoft", LogLevel.Error);
        });
        
            //config.AddJsonFile(ConfigFile, optional: false, reloadOnChange: true);
        
        //database
        var dbConfig = builder.Configuration.GetSection(nameof(DatabaseConfiguration)).Get<DatabaseConfiguration>();
        var emailOptions = builder.Configuration.GetSection(nameof(EmailOptions)).Get<EmailOptions>();
        
        var local_path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var entryAssemblyName = Assembly.GetEntryAssembly()?.GetName()?.Name ?? "StockDrops";
        var localAppFolder = Path.Combine(local_path, $"StockDrops\\{entryAssemblyName}");

        dbConfig.ConnectionString = dbConfig.ConnectionString?.Replace("[path]", Path.Combine(localAppFolder, dbConfig.DatabaseName!));
        emailOptions.ConnectionString = emailOptions.ConnectionString?.Replace("[path]", Path.Combine(localAppFolder, emailOptions.DatabaseName!));
        
        //Create the folder if it doesn't exist.
        Directory.CreateDirectory(localAppFolder);


        builder.Services.Configure<DatabaseConfiguration>(options =>
        {
            options.DatabaseName = dbConfig.DatabaseName;
            options.ConnectionString = dbConfig.ConnectionString;
        });
        builder.Services.Configure<EmailOptions>(options =>
        {
            options.DatabaseName = emailOptions.DatabaseName;
            options.EncryptionKey = emailOptions.EncryptionKey;
            options.ConnectionString = emailOptions.ConnectionString;
        });


        builder.Services.AddSingleton<ApplicationHostService>();

        

        builder.Services.AddDbContext<AppDbContext>();
        builder.Services.AddDbContextFactory<AppDbContext>();
        builder.Services.AddDbContext<EmailDbContext>();
        builder.Services.AddDbContextFactory<EmailDbContext>();
        builder.Services.AddMemoryCache();

        builder.Services.AddHttpClient<DiscordWebhookService>(client =>
       {
           client.DefaultRequestHeaders.UserAgent.TryParseAdd("StockDrops OpenStockApp/1.0");
       });

        builder.Services.AddSingleton<ISettingsService, SettingsService>();

        builder.Services.Configure<AzureClientConfiguration>(builder.Configuration.GetSection(nameof(AzureClientConfiguration)));
        builder.Services.AddSingleton<IIdentityService, IdentityService>();
        builder.Services.AddSingleton<IDeviceService, DeviceService>();



        #region Hubs
#if DEBUG
        builder.Services.Configure<NotificationsHubClientConfiguration>(builder.Configuration.GetSection($"{nameof(NotificationsHubClientConfiguration)}Debug"));
        builder.Services.Configure<BaseHubConfiguration>(builder.Configuration.GetSection($"{nameof(BaseHubConfiguration)}Debug"));
#else
        builder.Services.Configure<NotificationsHubClientConfiguration>(builder.Configuration.GetSection(nameof(NotificationsHubClientConfiguration)));
        builder.Services.Configure<BaseHubConfiguration>(builder.Configuration.GetSection(nameof(BaseHubConfiguration)));
#endif



        builder.Services.AddSingleton<NotificationsHubClient>();
        builder.Services.AddSingleton<INotificationsHubClient>(services => services.GetRequiredService<NotificationsHubClient>());
        builder.Services.AddSingleton<IBaseHubClient, NotificationsHubClient>(services => services.GetRequiredService<NotificationsHubClient>());

        

        //TODO: Add Autofac to simplify all of this.
        builder.Services.AddSingleton<UserOptionsHubClient>();
        builder.Services.AddSingleton<IUserOptionsHubClient, UserOptionsHubClient>(services => services.GetRequiredService<UserOptionsHubClient>());
        builder.Services.AddSingleton<IBaseHubClient, UserOptionsHubClient>(services => services.GetRequiredService<UserOptionsHubClient>());


#endregion

        builder.Services.AddSingleton<IUserOptionsService, UserOptionsService>();
        builder.Services.AddSingleton<IUserOptionsDisplayService, UserOptionsDisplayService>();
        builder.Services.AddSingleton<IRetailerOptionsDisplayService, RetailerOptionsDisplayService>();
        builder.Services.AddSingleton<IFilterService, FilterService>();
        //subs
        builder.Services.AddSingleton<ISubscriptionService, SubscriptionService>();

        //discord:
        builder.Services.AddSingleton<DiscordWebhookService>();
        builder.Services.AddSingleton<IDiscordWebhookService, DiscordWebhookService>(services => services.GetRequiredService<DiscordWebhookService>());
        builder.Services.AddSingleton<INotificationService, DiscordWebhookService>(services => services.GetRequiredService<DiscordWebhookService>());

        var apiConfig = builder.Configuration.GetSection(nameof(ApiConfiguration)).Get<ApiConfiguration>();
        if(apiConfig == null)
            throw new ArgumentNullException(nameof(ApiConfiguration));




        builder.Services.AddHttpClient<IApiService, ApiService>()
            .ConfigureHttpClient(httpClient =>
            {
                if(!string.IsNullOrEmpty(apiConfig.ApiBaseUrl))
                    httpClient.BaseAddress = new Uri(apiConfig.ApiBaseUrl);
            });
        builder.Services.AddSingleton<IEntityApiService<OpenStockApi.Core.Models.Users.User>, EntityApiService<OpenStockApi.Core.Models.Users.User>>(services =>
        {
            var apiService = services.GetRequiredService<IApiService>();
            return new EntityApiService<OpenStockApi.Core.Models.Users.User>(apiService, "/Users");
        });

        builder.Services.AddSingleton<IEntityApiService<OpenStockApi.Core.Models.Users.Device>, EntityApiService<OpenStockApi.Core.Models.Users.Device>>(services =>
        {
            var apiService = services.GetRequiredService<IApiService>();
            return new EntityApiService<OpenStockApi.Core.Models.Users.Device>(apiService, "/Devices");
        });

        builder.Services.AddSingleton<IEntityApiService<OpenStockApi.Core.Models.Users.Subscription>, EntityApiService<OpenStockApi.Core.Models.Users.Subscription>>(services =>
        {
            var apiService = services.GetRequiredService<IApiService>();
            return new EntityApiService<OpenStockApi.Core.Models.Users.Subscription>(apiService, "/Subscriptions");
        });

        builder.Services.AddSingleton<IUserEntityApiService<OpenStockApi.Core.Models.Users.UserOptions>, SelfEntityApiService<OpenStockApi.Core.Models.Users.UserOptions>>(services =>
        {
            var apiService = services.GetRequiredService<IApiService>();
            return new SelfEntityApiService<OpenStockApi.Core.Models.Users.UserOptions>(apiService, "/UserOptions");
        });

        builder.Services.AddSingleton<IEntityApiService<OpenStockApi.Core.Models.Users.ModelOptions>, EntityApiService<OpenStockApi.Core.Models.Users.ModelOptions>>(services =>
        {
            var apiService = services.GetRequiredService<IApiService>();
            return new EntityApiService<OpenStockApi.Core.Models.Users.ModelOptions>(apiService, "/ModelOptions");
        });


        var legacyApiConfig = builder.Configuration.GetSection("LegacyApiConfiguration").Get<ApiConfiguration>();
        if (legacyApiConfig == null)
            throw new ArgumentNullException("LegacyApiConfiguration");

        //var cert = CertHelper.GetCertificate(certPem, keyPem);
        var cert = X509Certificate2.CreateFromPem(legacyApiConfig.CertificatePublicKey, legacyApiConfig.CertificatePrivateKey);

        //this is required due to a bug in Windows detailed here: https://github.com/dotnet/runtime/issues/23749#issuecomment-739888661
        cert = new X509Certificate2(cert.Export(X509ContentType.Pkcs12));


        builder.Services.AddHttpClient<LegacyApiService>()
            .ConfigurePrimaryHttpMessageHandler(() =>
            {
                var httpClientHandler = new HttpClientHandler();
                httpClientHandler.CookieContainer = new CookieContainer();
                httpClientHandler.AutomaticDecompression = DecompressionMethods.All;
                httpClientHandler.UseCookies = true;
                //this SSL check should only happen in non debug code or else we might break when testing with localhost, or debugging with Fiddler.
//#if !DEBUG
//                    httpClientHandler.ServerCertificateCustomValidationCallback += ServerCertificateNoSnoopingAllowedValidation;
//#endif
                return httpClientHandler;
            })
            .ConfigureHttpClient
            (
                client =>
                {
                    client.BaseAddress = new Uri(legacyApiConfig.ApiBaseUrl ?? throw new ArgumentNullException("Legacy base url cannot be null"));
                }
            )
            .AddClientCertificate(cert);
        //builder.Services.AddSingleton<LegacyApiService>();

        //builder.Services.AddSingleton<IThemeSelectorService, ThemeSelectorService>();

        builder.Services.AddSingleton<IUserOptionsService, OpenStockApp.Services.UserOptionsService>();
        builder.Services.AddSingleton<IResultWindowService, ResultWindowService>();
        builder.Services.AddSingleton<INotificationHubService, NotificationHubService>();
        builder.Services.AddSingleton<INotificationService, NotificationActionService>();
#if WINDOWS
        builder.Services.AddSingleton<INotificationService, WindowsNotificationService>();
#elif ANDROID
        builder.Services.AddSingleton<INotificationService, AndroidNotificationService>();
#endif
        //configure
        builder.Services.Configure<AppConfig>(builder.Configuration.GetSection(nameof(AppConfig)));
        //Viewmodels
        builder.Services.AddTransient<AboutSettingsViewModel>();
        builder.Services.AddTransient<DiscordSettingsViewModel>();
        builder.Services.AddTransient<ThemeViewModel>();
        builder.Services.AddTransient<UserSettingsViewModel>();
        builder.Services.AddTransient<AlertSettingsViewModel>();
        builder.Services.AddTransient<UserOptionsViewModel>();
        builder.Services.AddTransient<NotificationsPageViewModel>();
        builder.Services.AddTransient<ActiveNotificationsViewModel>();
        //pages
        builder.Services.AddTransient<AlertSettingsPage>();
        builder.Services.AddTransient<ActiveNotificationsPage>();
        builder.Services.AddTransient<AboutSettingsPage>();
        builder.Services.AddTransient<NotificationsPage>();
#if ANDROID || IOS
        builder.Services.AddTransient<NotificationPageMobile>();
#endif
        var app = builder.Build();
        app.MigrateDatabase<AppDbContext>();
        app.MigrateDatabase<EmailDbContext>();
        return app;
	}
}
