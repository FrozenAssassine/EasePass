using Avalonia.Data.Converters;
using Avalonia.Media;
using EasePass.Controls;
using System;
using System.Globalization;

namespace EasePass.Converter;

public class SeverityToBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value switch
        {
            InfoBarSeverity.Info => new SolidColorBrush(Color.Parse("#0078D4")),
            InfoBarSeverity.Success => new SolidColorBrush(Color.Parse("#107C10")),
            InfoBarSeverity.Warning => new SolidColorBrush(Color.Parse("#FFAA00")),
            InfoBarSeverity.Error => new SolidColorBrush(Color.Parse("#D13438")),
            _ => new SolidColorBrush(Colors.Gray)
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
}
