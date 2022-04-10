using Microsoft.Toolkit.Mvvm.ComponentModel;
using OpenStockApi.Core.Models.Products;
using OpenStockApi.Core.Models.Users;
using System.Collections.ObjectModel;

namespace OpenStockApp.Core.Maui.Models
{
    public class ObservableModelOptions : ObservableObject
    {
        public Model Model { get; set; }
        public ModelOptions ModelOptions { get; set; }

        public ObservableModelOptions(Model model, ModelOptions modelOptions)
        {
            Model = model;
            ModelOptions = modelOptions;
        }

    }

    public class GroupedObversableModelOptions : ObservableCollection<ObservableModelOptions>
    {
        public string Name { get; private set; }
        public GroupedObversableModelOptions(string name, List<ObservableModelOptions> observableModelOptions) : base(observableModelOptions)
        {
            Name = name;
        }

    }
}
