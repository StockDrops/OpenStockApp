using Microsoft.Extensions.Logging;
using OpenStockApi.Core.Models.Searches;
using OpenStockApp.Core.Contracts.Services.Hubs;
using OpenStockApp.Core.Contracts.Services.Settings;
using OpenStockApp.Core.Maui.Contracts.Services;

namespace OpenStockApp.Core.Maui.Services.Notifications
{
    public class ResultWindowService : IResultWindowService
    {
        public event EventHandler<Result>? DisplayResult;
        public event EventHandler? EndReached;


        private readonly INotificationsHubClient notificationsHubClient;
        private readonly IUserOptionsService userOptionsService;
        private readonly ILogger logger;

        public ResultWindowService(INotificationsHubClient notificationsHubClient,
            IUserOptionsService userOptionsService,
            ILogger<ResultWindowService> logger)
        {
            this.notificationsHubClient = notificationsHubClient;
            this.userOptionsService = userOptionsService;
            this.logger = logger;
        }
        /// <summary>
        /// Loads the results from the server and triggers the events for different display.
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task LoadResultsFromServerAsync(int pageSize, int pageNumber, CancellationToken cancellationToken = default)
        {
            try
            {
                var count = 0;
                var stream = notificationsHubClient.GetLatestResults(pageSize, pageNumber, cancellationToken);
                if (!await stream.AnyAsync(cancellationToken))
                {
                    EndReached?.Invoke(this, EventArgs.Empty);
                    return;
                }
                await foreach (var result in stream)
                {
                    result.DateTimeFound = result.DateTimeFound.ToLocalTime();
                    DisplayResult?.Invoke(this, result);
                    count++;
                }
                if (count < pageSize)
                    EndReached?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "");
            }
        }

        public async Task LoadFilteredResultsFromServerAsync(int pageSize, int pageNumber, CancellationToken cancellationToken = default)
        {
            
            try
            {
                var count = 0;
                var stream = notificationsHubClient.GetFilteredResults(userOptionsService.UserOptions, pageSize, pageNumber, cancellationToken);

                if (!await stream.AnyAsync(cancellationToken))
                {
                    EndReached?.Invoke(this, EventArgs.Empty);
                    return;
                }
                await foreach (var result in stream)
                {
                    result.DateTimeFound = result.DateTimeFound.ToLocalTime();
#if IOS
                    await Task.Delay(50);
#endif
                    DisplayResult?.Invoke(this, result);
                    count++;
                }
                if (count < pageSize)
                    EndReached?.Invoke(this, EventArgs.Empty);
            }
            catch(Exception ex)
            {
                logger.LogError(ex, "");
            }
        }


        protected virtual void OnResultReadyToBeDisplayed(object? sender, Result result)
        {
            DisplayResult?.Invoke(sender, result);
        }
    }
}
