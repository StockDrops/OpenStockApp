using OpenStockApi.Core.Models.Users;
using OpenStockApp.Models.Users;
using System;
using System.Globalization;

namespace OpenStockApp.Converters
{
    public class DisplayedNotificationActionsToNotificationsActionConverter : IValueConverter
    {
        public BindableProperty EnumTypeProperty = BindableProperty.Create(nameof(EnumType), typeof(Type), typeof(DisplayedNotificationActionsToNotificationsActionConverter), null);
        public Type? EnumType { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is DisplayedNotificationActions action)
            {
                if (EnumType != null && Enum.IsDefined(EnumType, value))
                {
                    //var enumValue = Enum.Parse(EnumType, enumString);

                    return action.Action;
                }
            }
            
            

            return false;
        }

        public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DisplayedNotificationActions action)
            {
                return action.Action;
            }

            return NotificationAction.OpenProductUrl;
        }
    }
}
