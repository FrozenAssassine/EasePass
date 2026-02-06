using Avalonia.Data.Converters;
using EasePass.Controls;
using System;
using System.Globalization;

namespace EasePass.Converter
{
    internal class SeverityToIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value switch
            {
                InfoBarSeverity.Info => "ℹ️",
                InfoBarSeverity.Success => "✔️",
                InfoBarSeverity.Warning => "⚠️",
                InfoBarSeverity.Error => "❌",
                _ => ""
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();

    }
}
