using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using EasePass.Helper;
using EasePass.Settings;

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

        private PasswordSafetyChart PasswordSafetyChart;

        private string _Password;
        public new string Password
        {
            get => _Password;
            set
            {
                _Password = value;
                ToggleShowPassword(ShowPassword);
                if(PasswordSafetyChart != null)
                    PasswordSafetyChart.EvaluatePassword(_Password);
            }
        }

        private bool _ShowPassword = false;
        public bool ShowPassword { get => _ShowPassword; set { _ShowPassword = value; ToggleShowPassword(value); } }

        private void CopyText() => ClipboardHelper.Copy(this.Password);

        private void TextBox_DoubleTapped(object sender, Microsoft.UI.Xaml.Input.DoubleTappedRoutedEventArgs e)
        {
            if (AppSettings.GetSettingsAsBool(AppSettingsValues.doubleTapToCopy, DefaultSettingsValues.doubleTapToCopy))
                CopyText();
        }
        private void CopyText_Click(object sender, RoutedEventArgs e)
        {
            CopyText();
        }

        private void showPW_Toggled(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                ShowPassword = !ShowPassword;
                button.Content = ShowPassword ? '\uED1A' : '\uF78D';
            }
        }

        private void TextBox_PreviewKeyDown(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (KeyHelper.IsKeyPressed(Windows.System.VirtualKey.Control))
            {
                if (e.Key == Windows.System.VirtualKey.A)
                    this.SelectAll();
                else if (e.Key == Windows.System.VirtualKey.C)
                    CopyText();
                
                e.Handled = true;
                return;
            }
        }
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            PasswordSafetyChart = GetTemplateChild("passwordSafetyChart") as PasswordSafetyChart;
        }
    }
}
