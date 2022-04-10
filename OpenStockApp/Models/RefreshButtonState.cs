using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace OpenStockApp.Models
{
    public class RefreshButtonState : ObservableObject
    {
        private bool isVisible;
        public bool IsVisible
        {
            get => isVisible;
            set => SetProperty(ref isVisible, value);
        }
        private bool isEnabled;
        public bool IsEnabled
        {
            get => isEnabled;
            set => SetProperty(ref isEnabled, value);
        }
        private string? buttonText;
        public string? ButtonText
        {
            get => buttonText;
            set => SetProperty(ref buttonText, value);
        }
        /// <summary>
        /// This method allows you to timeout the state of a button binded to this model. It will change the button text, and set the IsEnabled to false
        /// during the timeout if <paramref name="disableButton"/> is set to true.
        /// </summary>
        /// <param name="timeoutText"></param>
        /// <param name="regularText"></param>
        /// <param name="milliseconds"></param>
        /// <param name="disableButton"></param>
        /// <returns></returns>
        public async Task TimeoutButton(string timeoutText, string regularText, int milliseconds, bool disableButton = true)
        {
            ButtonText = timeoutText;
            if (disableButton)
                IsEnabled = false;
            await Task.Delay(milliseconds);
            ButtonText = regularText;
            if (disableButton)
                IsEnabled = true;
        }
    }
}
