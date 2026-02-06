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

using System;
using System.Security;

namespace EasePass.Extensions;

/// <summary>
/// Includes every Extension for the <see cref="string"/>
/// </summary>
public static class StringExtension
{
    /// <summary>
    /// Converts the given <paramref name="value"/> to a <see cref="byte"/>[]
    /// </summary>
    /// <param name="value">The Value, which should be converted</param>
    /// <returns>Returns the <paramref name="value"/> as <see cref="byte"/>[]</returns>
    public static byte[] ConvertToBytes(this string value)
    {
        return value.AsSpan().ConvertToBytes();
    }

    /// <summary>
    /// Converts the given <paramref name="plainString"/> to a <see cref="SecureString"/>
    /// </summary>
    /// <param name="plainString">The <see cref="string"/>, which should be converted</param>
    /// <returns>Returns the <paramref name="plainString"/> as <see cref="SecureString"/></returns>
    public static SecureString ConvertToSecureString(this string plainString)
    {
        return plainString.AsSpan().ConvertToSecureString();
    }
}
