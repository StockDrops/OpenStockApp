using OpenStockApi.Core.Models.Products;
using OpenStockApp.Core.Contracts.Services.Hubs;
using OpenStockApp.Core.Contracts.Services.Settings;
using OpenStockApp.Core.Maui.Models;
using System.Diagnostics;

namespace OpenStockApp.Core.Maui.Services.Settings
{
    public class UserOptionsDisplayService : IUserOptionsDisplayService
    {
        private readonly IUserOptionsHubClient userOptionsHubClient;
        private readonly IUserOptionsService userOptionsService;
        public UserOptionsDisplayService(IUserOptionsHubClient userOptionsHubClient,
            IUserOptionsService userOptionsService)
        {
            this.userOptionsHubClient = userOptionsHubClient;
            this.userOptionsService = userOptionsService;
        }

        public async Task<IList<GroupedObversableModelOptions>> GetGroupedObversableModelOptionsAsync(Product product, CancellationToken cancellationToken)
        {

            var observableModelOptions = await GetObservableModelOptions(product, cancellationToken);
            
            var models = observableModelOptions
                .GroupBy(m => m.Model.BrandId)
                .OrderBy(g => g.First().Model?.Brand?.Name)
                .Select(x => new GroupedObversableModelOptions(x.First().Model?.Brand?.Name ?? "Null", x.ToList()))
                .ToList();

            return models;
        }




        public async Task<IList<ShowModel>> GetObservableModelOptions(Product product, CancellationToken cancellationToken = default)
        {
            var models = await userOptionsHubClient.GetModels(product, cancellationToken);
            var observableModels = new List<ShowModel>();
            var options = userOptionsService.UserOptions;


            foreach (var model in models)
            {
                var modelOption = options?.ModelOptions?.Where(m => m.ModelId == model.Id).FirstOrDefault();

                if (options != null && modelOption != null)
                {
                    observableModels.Add(new ShowModel(model, modelOption));
                }
                else
                {
                    modelOption = new OpenStockApi.Core.Models.Users.ModelOptions
                    {
                        ModelId = model.Id

                    };
                    options?.ModelOptions.Add(modelOption);
                    observableModels.Add(new ShowModel(model, modelOption));
                }
            }

            return observableModels;
        }
    }
}
