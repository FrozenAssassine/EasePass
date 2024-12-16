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

using EasePass.Helper;
using Microsoft.UI.Xaml.Data;
using System;

namespace EasePass.Converter;

internal class NullToVisibilityConverter_Inverted : IValueConverter
{
    public object Convert(object value, Type targetType,
        object parameter, string language)
    {
        return ConvertHelper.BoolToVisibility(value == null);
    }

    public object ConvertBack(object value, Type targetType,
        object parameter, string language)
    {
        throw new NotImplementedException();
    }
}
