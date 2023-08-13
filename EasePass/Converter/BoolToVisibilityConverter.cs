using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasePass.Converter
{
    internal class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, string language)
        {
            if (value == null)
                return Visibility.Visible;

            return (bool.TryParse(value.ToString(), out bool res)) && res == true ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, string language)
        {
            if (value == null)
                return false;

            return (Enum.TryParse(typeof(Visibility), value.ToString(), out object res)) && (Visibility)res == Visibility.Visible;
        }
    }
}
