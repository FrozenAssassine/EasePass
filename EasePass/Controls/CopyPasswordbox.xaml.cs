using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using EasePass.Helper;

namespace EasePass.Controls
{
    public sealed partial class CopyPasswordbox : TextBox
    {
        public CopyPasswordbox()
        {
            this.InitializeComponent();
            this.IsReadOnly = true;
        }

        private void ToggleShowPassword(bool show)
        {
            base.Text = show ? _Password : new string('•', _Password.Length);
        }


        private string _Password;
        public new string Text
        {
            get => _Password;
            set
            {
                _Password = value;
                ToggleShowPassword(ShowPassword);
            }
        }


        private bool _ShowPassword = false;
        public bool ShowPassword { get => _ShowPassword; set { _ShowPassword = value; ToggleShowPassword(value); } }

        private void CopyText_Click(object sender, RoutedEventArgs e)
        {
            ClipboardHelper.Copy(this._Password);
        }

        private void showPW_Toggled(object sender, RoutedEventArgs e)
        {
            if(sender is CheckBox cb)
            {
                ToggleShowPassword(cb.IsChecked ?? false);
            }
        }

        private void TextBox_PreviewKeyDown(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (KeyHelper.IsKeyPressed(Windows.System.VirtualKey.Control))
            {
                if (e.Key == Windows.System.VirtualKey.A)
                    this.SelectAll();
                else if (e.Key == Windows.System.VirtualKey.C)
                    CopyText_Click(null, null);
                
                e.Handled = true;
                return;
            }
        }
    }
}
