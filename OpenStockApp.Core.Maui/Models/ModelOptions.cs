using Microsoft.Toolkit.Mvvm.ComponentModel;
using OpenStockApi.Core.Models.Products;
using OpenStockApi.Core.Models.Users;
using System.Collections.ObjectModel;

namespace OpenStockApp.Core.Maui.Models
{
    public class ObservableModelOptions : ObservableObject
    {
        private readonly ModelOptions modelOptions;
        public ModelOptions ModelOptions => modelOptions;

        public ObservableModelOptions(ModelOptions modelOptions)
        {
            this.modelOptions = modelOptions;
        }
        public bool IsEnabled
        {
            get { return modelOptions.IsEnabled; }
            set { SetProperty(ModelOptions.IsEnabled, value, ModelOptions, (m, n) => m.IsEnabled = n); }
        }
        public NotificationAction NotificationAction
        {
            get => modelOptions.NotificationAction;
            set => SetProperty(ModelOptions.NotificationAction, value, ModelOptions, (m, n) => m.NotificationAction = n);
        }
        public decimal? MinPrice
        {
            get => modelOptions.MinPrice;
            set => SetProperty(ModelOptions.MinPrice, value, ModelOptions, (m, n) => m.MinPrice = n);
        }
        public decimal? MaxPrice
        {
            get => modelOptions.MaxPrice;
            set => SetProperty(ModelOptions.MaxPrice, value, ModelOptions, (m, n) => m.MaxPrice = n);
        }
    }

    public class ShowModel : ObservableObject
    {
        public Model Model { get; set; }
        public ObservableModelOptions ModelOptions { get; set; }

        public ShowModel(Model model, ModelOptions modelOptions)
        {
            Model = model;
            ModelOptions = new ObservableModelOptions(modelOptions);
        }
    }

    public class GroupedObversableModelOptions : ObservableCollection<ShowModel>
    {
        public string Name { get; private set; }
        public GroupedObversableModelOptions(string name, List<ShowModel> observableModelOptions) : base(observableModelOptions)
        {
            Name = name;
        }

    }
}
