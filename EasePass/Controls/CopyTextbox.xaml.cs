using EasePass.Helper;
using Microsoft.UI.Xaml.Controls;

namespace EasePass.Controls
{
    public sealed partial class CopyTextbox : TextBox
    {
        public CopyTextbox()
        {
            this.InitializeComponent();
        }

        private void CopyText_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            ClipboardHelper.Copy(this.Text);
        }
    }
}
