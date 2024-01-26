using EasePass.Helper;
using Microsoft.UI.Xaml.Data;
using System;

namespace EasePass.Converter;

public class NullToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType,
        object parameter, string language)
    {
        return ConvertHelper.BoolToVisibility(value != null);
    }

    public object ConvertBack(object value, Type targetType,
        object parameter, string language)
    {
        throw new NotImplementedException();
    }
}
