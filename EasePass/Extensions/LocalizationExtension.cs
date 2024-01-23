namespace EasePass.Extensions;

public static class LocalizationExtension
{
    public static string Localized(this string originalString, string localizeValue)
    {
        var res = MainWindow.localizationHelper.resourceMap.TryGetSubtree("Resources")?.TryGetValue(localizeValue, MainWindow.localizationHelper.resourceContext);
        if (res == null)
            return originalString;

        return res.ValueAsString;
    }
}
