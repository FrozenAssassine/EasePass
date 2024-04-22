using EasePass.Helper;
using EasePass.Settings;
using Microsoft.UI.Xaml.Controls;
using System;

namespace EasePass.Controls
{
    public sealed partial class CopyTextbox : TextBox
    {
        public bool RemoveWhitespaceOnCopy { get; set; } = false;
        public bool IsUrlAction { get; set; } = false;

        public CopyTextbox()
        {
            this.InitializeComponent();
        }

        private async void CopyText_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            string txt = this.Text;
            if (!txt.ToLower().StartsWith("http")) txt = "http://" + txt;
            if (string.IsNullOrEmpty(txt))
                return;
            try
            {
                if (IsUrlAction)
                {
                    await Windows.System.Launcher.LaunchUriAsync(new Uri(txt));
                    return;
                }
            }
            catch (UriFormatException) { /*Invalid URL*/ return; }

            ClipboardHelper.Copy(RemoveWhitespaceOnCopy ? this.Text.Replace(" ", "") : this.Text);
        }

        private void TextBox_DoubleTapped(object sender, Microsoft.UI.Xaml.Input.DoubleTappedRoutedEventArgs e)
        {
            if (AppSettings.GetSettingsAsBool(AppSettingsValues.doubleTapToCopy, DefaultSettingsValues.doubleTapToCopy))
                CopyText_Click(this, null);
        }
    }
}
