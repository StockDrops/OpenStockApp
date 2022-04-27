namespace OpenStockApp.ViewModels
{
    public interface IBaseViewModel
    {
        bool IsBusy { get; set; }
        string? Title { get; set; }
    }
}