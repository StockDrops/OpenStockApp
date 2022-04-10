using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenStockApp.Models
{
    public class ConfirmationMessage : BindableObject
    {
        public static readonly BindableProperty MessageProperty =
            BindableProperty.Create(nameof(Message), typeof(string), typeof(ConfirmationMessage), "");
        public static readonly BindableProperty IsVisibleProperty =
            BindableProperty.Create(nameof(IsVisible), typeof(bool), typeof(ConfirmationMessage), false);
        public static readonly BindableProperty IsSuccessProperty =
            BindableProperty.Create(nameof(IsSuccess), typeof(bool), typeof(ConfirmationMessage), false);
        public string Message
        {
            get => (string)GetValue(MessageProperty);
            set => SetValue(MessageProperty, value);
        }
        public bool IsVisible
        {
            get => (bool)GetValue(IsVisibleProperty);
            set => SetValue(IsVisibleProperty, value);
        }
        public bool IsSuccess
        {
            get => (bool)GetValue(IsSuccessProperty);
            set => SetValue(IsSuccessProperty, value);
        }
    }
}
