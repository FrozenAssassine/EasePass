using Microsoft.Windows.ApplicationModel.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasePass.Helper
{
    internal class LocalizationHelper
    {
        public static string GetString(string key)
        {
            ResourceManager resourceManager = new ResourceManager();
            return resourceManager.MainResourceMap.GetValue(key).ValueAsString;
        }

        public static void ChangeLanguage(string language)
        {
            Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride = language;
        }


        public IReadOnlyList<string> Languages => Windows.Globalization.ApplicationLanguages.Languages;
    }
}
