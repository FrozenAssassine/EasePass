using Microsoft.UI.Xaml;

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
    }
}
