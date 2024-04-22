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
