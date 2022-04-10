using Microsoft.Toolkit.Uwp.Notifications;
using OpenStockApi.Core.Models.Searches;
using OpenStockApp.Core.Maui.Resources.Strings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Windows.UI.Notifications;

namespace OpenStockApp.Core.Maui.Platforms.Windows.Notifications
{
    public class NotificationBuilder
    {
        
        public NotificationBuilder()
        {
            toastContentBuilder = new ToastContentBuilder();
        }
        private ToastContentBuilder toastContentBuilder;
        public NotificationBuilder WithResult(Result result)
        {
            var buttons = GetToastButtons(result);
            toastContentBuilder.AddToastActivationInfo("", ToastActivationType.Foreground);
            if(result.Sku != null)
            {
                if(result.Sku.Model != null && !string.IsNullOrEmpty(result.Sku.Model.Name))
                    toastContentBuilder.AddText(string.Format(NotificationResources.ToastText, result.Sku.Model.Name));
                else
                    toastContentBuilder.AddText(string.Format(NotificationResources.ToastText, result.Sku.Name));
            }
            foreach(var button in buttons)
            {
                toastContentBuilder.AddButton(button);
            }
            
            return this;
        }
        public NotificationBuilder WithText(string text)
        {
            toastContentBuilder.AddText(text);
            return this;
        }
        public ToastContentBuilder Build()
        {
            return toastContentBuilder;
        }
        public void Show()
        {
            toastContentBuilder
                
                .Show();
        }
        private IToastButton[] GetToastButtons(Result result)
        {
            IToastButton[] toastButtons = null;
            if (!string.IsNullOrEmpty(result.AtcUrl))
            {
               
                var productUrlButton = new ToastButton(NotificationResources.NotificationAddToCartButton, $"url={HttpUtility.UrlEncodeUnicode(result.AtcUrl)}");
                productUrlButton.ActivationType = ToastActivationType.Foreground;
                
                toastButtons = new IToastButton[] { productUrlButton };
            }
            else if (!string.IsNullOrEmpty(result.ProductUrl))
            {
               
                var productUrlButton = new ToastButton(NotificationResources.NotificationProductUrlButton, $"url={HttpUtility.UrlEncodeUnicode(result.ProductUrl)}");
                productUrlButton.ActivationType = ToastActivationType.Foreground;
                toastButtons = new IToastButton[] { productUrlButton };
            }
            return toastButtons;
        }
    }
}
