using EasePass.Helper;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Diagnostics;

namespace EasePass.Controls
{
    public sealed partial class CopyTextbox : TextBox
    {
        public bool RemoveWhitespaceOnCopy = false;
        public bool IsUrlAction = false;

        public CopyTextbox()
        {
            this.InitializeComponent();
        }

        private async void CopyText_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            string txt = this.Text;
            if (!txt.ToLower().StartsWith("http")) txt = "http://" + txt;
            if (IsUrlAction && !string.IsNullOrEmpty(txt)) await Windows.System.Launcher.LaunchUriAsync(new Uri(txt));
            if (RemoveWhitespaceOnCopy)
            {
                ClipboardHelper.Copy(this.Text.Replace(" ", ""));
            }
            else
            {
                ClipboardHelper.Copy(this.Text);
            }
        }
    }
}
