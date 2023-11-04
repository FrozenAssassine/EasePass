using Microsoft.UI.Xaml;
using System;

namespace EasePass.Helper
{
    internal class ConvertHelper
    {
        public static Visibility BoolToVisibility(bool visible)
        {
            return visible ? Visibility.Visible : Visibility.Collapsed;
        }

        public static int ToInt(object value, int defaultValue = 0)
        {
            if (value != null)
            {
                if (int.TryParse(value.ToString(), out int converted) == true)
                    return converted;
            }
            return defaultValue;
        }

        public static bool ToBoolean(object value, bool defaultValue = false)
        {
            if (value != null)
            {
                if (bool.TryParse(value.ToString(), out bool converted) == true)
                    return converted;
            }
            return defaultValue;
        }

        /// <summary>
        /// Converts the given input to an enum of the given type. When it fails it returns the defaultValue
        /// </summary>
        /// <typeparam name="TEnum">The type of enum</typeparam>
        /// <param name="value">The value to convert to enum</param>
        /// <param name="defaultValue">The value to return when an error occurs</param>
        /// <returns>An enum of the given type or the default value</returns>
        public static TEnum ToEnum<TEnum, T>(T value, TEnum defaultValue) where TEnum : struct
        {
            if (value != null)
            {
                if (value is TEnum res)
                    return res;

                if (Enum.TryParse(typeof(TEnum), value.ToString(), out object? result))
                    return result == null ? defaultValue : (TEnum)result;
            }
            return defaultValue;
        }
    }
}
