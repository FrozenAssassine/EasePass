using EasePass.Helper;
using EasePass.Settings;
using Microsoft.UI.Xaml.Controls;

namespace EasePass.Controls
{
    public sealed partial class CopyTextbox : TextBox
    {
        public bool RemoveWhitespaceOnCopy { get; set; } = false;

        public CopyTextbox()
        {
            this.InitializeComponent();
        }

        private void CopyText_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            ClipboardHelper.Copy(RemoveWhitespaceOnCopy ? this.Text.Replace(" ", "") : this.Text);            
        }

        private void TextBox_DoubleTapped(object sender, Microsoft.UI.Xaml.Input.DoubleTappedRoutedEventArgs e)
        {
            if (AppSettings.GetSettingsAsBool(AppSettingsValues.doubleTapToCopy, DefaultSettingsValues.doubleTapToCopy))
                ClipboardHelper.Copy(this.Text);
        }
    }
}
