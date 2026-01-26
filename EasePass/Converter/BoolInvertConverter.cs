using Microsoft.UI.Xaml.Data;
using System;

namespace EasePass.Converter
{
    internal partial class BoolInvertConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, string language)
        {
            if (value == null)
                return false;
            return !(bool)value;
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
