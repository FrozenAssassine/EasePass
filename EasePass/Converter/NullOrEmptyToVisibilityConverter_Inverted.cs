using EasePass.Helper;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;

namespace Fastedit.Converter
{
    public class NullOrEmptyToVisibilityConverter_Inverted : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, string language)
        {
            return ConvertHelper.BoolToVisibility(string.IsNullOrEmpty(value as string));
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

}
