using EasePass.Helper;
using EasePass.Settings;
using Microsoft.UI.Xaml.Controls;

namespace EasePass.Controls
{
    public sealed partial class EditPasswordBox : TextBox
    {
        public EditPasswordBox()
        {
            this.InitializeComponent();
        }

        public delegate void PasswordChangedEvent(string password);
        public event PasswordChangedEvent PasswordChanged;

        public string Password
        {
            get => base.Text;
            set => base.Text = value;
        }

        private void Textbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            PasswordChanged?.Invoke(Password);
        }

        private void CopyText() => ClipboardHelper.Copy(this.Password);

        private void TextBox_DoubleTapped(object sender, Microsoft.UI.Xaml.Input.DoubleTappedRoutedEventArgs e)
        {
            if (AppSettings.GetSettingsAsBool(AppSettingsValues.doubleTapToCopy, DefaultSettingsValues.doubleTapToCopy))
                CopyText();
        }

        private void TextBox_PreviewKeyDown(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (KeyHelper.IsKeyPressed(Windows.System.VirtualKey.Control) && !KeyHelper.IsKeyPressed(Windows.System.VirtualKey.Menu))
            {
                if (e.Key == Windows.System.VirtualKey.A)
                    this.SelectAll();
                else if (e.Key == Windows.System.VirtualKey.C)
                    CopyText();
                else if (e.Key == Windows.System.VirtualKey.V)
                    this.PasteFromClipboard();

                e.Handled = true;
                return;
            }
        }
    }
}
