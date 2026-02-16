using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace EasePass.Converter
{
    public class PercentageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double d && double.Parse(parameter?.ToString() ?? "1") is double p)
            {
                return d * p;
            }
            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
