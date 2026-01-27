using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI;
using DatabaseAvailability = EasePassExtensibility.IDatabaseSource.DatabaseAvailability;

namespace EasePass.Converter;

internal sealed class AvailabilityToBrushConverter : IValueConverter
{
    private static readonly Color LightAvailableColor = Color.FromArgb(255, 0, 0, 0);
    private static readonly Color LightUnknownColor = Color.FromArgb(255, 255, 200, 0);
    private static readonly Color LightErrorColor = Color.FromArgb(255, 255, 0, 0);

    private static readonly Color DarkAvailableColor = Color.FromArgb(255, 255, 255, 255);
    private static readonly Color DarkUnknownColor = Color.FromArgb(255, 255, 255, 0);
    private static readonly Color DarkErrorColor = Color.FromArgb(255, 255, 80, 80);

    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is not DatabaseAvailability availability)
            return null;

        return new SolidColorBrush(GetColor(availability, Application.Current.RequestedTheme));
    }

    private static Color GetColor(DatabaseAvailability availability, ApplicationTheme theme)
    {
        bool isDark = theme == ApplicationTheme.Dark;

        return availability switch
        {
            DatabaseAvailability.Available
                => isDark ? DarkAvailableColor : LightAvailableColor,

            DatabaseAvailability.UnknownState
                => isDark ? DarkUnknownColor : LightUnknownColor,

            DatabaseAvailability.LockedByOtherUser
            or DatabaseAvailability.Unavailable
                => isDark ? DarkErrorColor : LightErrorColor,

            _ => Colors.Transparent
        };
    }
    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}
