namespace EasePass.Extensions;

public static class LocalizationExtension
{
    public static string Localized(this string originalString, string localizeValue)
    {
        //return MainWindow.localizationHelper.resourceMap.GetValue("" + localizeValue).ValueAsString;
        
        var res = MainWindow.localizationHelper.resourceMap.TryGetValue(localizeValue, MainWindow.localizationHelper.resourceContext);
        if (res == null)
            return originalString;

        return res.ValueAsString;
    }
}
