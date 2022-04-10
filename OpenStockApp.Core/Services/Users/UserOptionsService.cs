using OpenStockApp.Core.Contracts.Services.Api;
using OpenStockApi.Core.Models.Users;
using OpenStockApp.Core.Services.Api;
using OpenStockApp.Core.Contracts.Services.Settings;

namespace OpenStockApp.Core.Services.Users
{
    public class OldUserOptionsService
    {
        private readonly IUserEntityApiService<UserOptions> entityApiService;
        public OldUserOptionsService(IUserEntityApiService<OpenStockApi.Core.Models.Users.UserOptions> entityApiService)
        {
            this.entityApiService = entityApiService ?? throw new ArgumentNullException(nameof(entityApiService));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<UserOptions?> GetUserOptionsAsync(CancellationToken cancellationToken = default)
        {
            var userOptions = await entityApiService.GetSelfEntityAsync(cancellationToken);
            return userOptions;
        }
    }
}
