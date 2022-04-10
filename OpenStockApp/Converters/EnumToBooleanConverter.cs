using System;
using System.Globalization;

namespace OpenStockApp.Converters
{
    public class EnumToBooleanConverter : IValueConverter
    {
        public Type? EnumType { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter is string enumString)
            {
                if (EnumType != null && Enum.IsDefined(EnumType, value))
                {
                    var enumValue = Enum.Parse(EnumType, enumString);

                    return enumValue.Equals(value);
                }
            }
            return false;
        }

        public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool convert && convert && parameter is string enumString && EnumType != null)
            {
                return Enum.Parse(EnumType, enumString);
            }

            return null;
        }
    }
}
