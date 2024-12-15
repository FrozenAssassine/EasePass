/*
MIT License

Copyright (c) 2023 Julius Kirsch

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.
*/

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
