using EasePass.Helper;
using Microsoft.UI.Xaml.Controls;

namespace EasePass.Controls
{
    public sealed partial class CopyTextbox : TextBox
    {
        public bool RemoveWhitespaceOnCopy = false;

        public CopyTextbox()
        {
            this.InitializeComponent();
        }

        private void CopyText_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            if(RemoveWhitespaceOnCopy)
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
