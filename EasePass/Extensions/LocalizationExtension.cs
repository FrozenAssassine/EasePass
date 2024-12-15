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

namespace EasePass.Extensions;

public static class LocalizationExtension
{
    public static string Localized(this string originalString, string localizeValue)
    {
        var res = MainWindow.localizationHelper.resourceMap.TryGetSubtree("Resources")?.TryGetValue(localizeValue, MainWindow.localizationHelper.resourceContext);
        if (res == null)
            return originalString;

        if (res.ValueAsString.Contains("\\n"))
            return res.ValueAsString.Replace("\\n", Environment.NewLine);
        return res.ValueAsString;
    }
}
