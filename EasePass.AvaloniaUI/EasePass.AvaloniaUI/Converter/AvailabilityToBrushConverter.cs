using Avalonia.Data.Converters;
using Avalonia.Media;
using EasePassExtensibility;
using System;
using System.Globalization;

namespace EasePass.Converter
{
    public class AvailabilityToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is IDatabaseSource.DatabaseAvailability availability)
            {
                switch (availability)
                {
                    case IDatabaseSource.DatabaseAvailability.Available:
                        return Brushes.LightGreen;
                    case IDatabaseSource.DatabaseAvailability.LockedByOtherUser:
                        return Brushes.Orange;
                    case IDatabaseSource.DatabaseAvailability.Unavailable:
                        return Brushes.Red;
                    case IDatabaseSource.DatabaseAvailability.UnknownState:
                        return Brushes.Gray;
                }
            }
            return Brushes.Gray;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
