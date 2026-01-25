using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.Text;

namespace EasePass.Converter
{
    internal class AvailabilityToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
                return null;

            if (value is not EasePassExtensibility.IDatabaseSource.DatabaseAvailability availability)
                return null;

            switch (availability)
            {
                case EasePassExtensibility.IDatabaseSource.DatabaseAvailability.Available:
                    return new SolidColorBrush(Windows.UI.Color.FromArgb(255, 255, 255, 255));
                case EasePassExtensibility.IDatabaseSource.DatabaseAvailability.UnknownState:
                    return new SolidColorBrush(Windows.UI.Color.FromArgb(255, 255, 255, 0));
                case EasePassExtensibility.IDatabaseSource.DatabaseAvailability.LockedByOtherUser:
                    return new SolidColorBrush(Windows.UI.Color.FromArgb(255, 255, 0, 0));
                case EasePassExtensibility.IDatabaseSource.DatabaseAvailability.Unavailable:
                    return new SolidColorBrush(Windows.UI.Color.FromArgb(255, 255, 0, 0));
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
