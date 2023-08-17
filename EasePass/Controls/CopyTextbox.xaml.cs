using EasePass.Helper;
using EasePass.Settings;
using Microsoft.UI.Xaml.Controls;

namespace EasePass.Controls
{
    public sealed partial class CopyTextbox : TextBox
    {
        public CopyTextbox()
        {
            this.InitializeComponent();
        }

        private void CopyText() => ClipboardHelper.Copy(this.Text);

        private void CopyText_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            CopyText();
        }

        private void TextBox_DoubleTapped(object sender, Microsoft.UI.Xaml.Input.DoubleTappedRoutedEventArgs e)
        {
            if (AppSettings.GetSettingsAsBool(AppSettingsValues.doubleTapToCopy, DefaultSettingsValues.doubleTapToCopy))
                CopyText();
        }
    }
}
