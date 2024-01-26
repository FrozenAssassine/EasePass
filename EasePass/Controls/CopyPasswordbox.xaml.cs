using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using EasePass.Helper;
using EasePass.Settings;
using EasePass.Core;
using System.Collections.ObjectModel;
using EasePass.Models;

namespace EasePass.Controls
{
    public sealed partial class CopyPasswordbox : TextBox
    {
        private PasswordSafetyChart pwSafetyChart = new PasswordSafetyChart
        {
            Margin = new Thickness(6, 2, 5, 2),
            Height = 30,
            ShowInfo = false,
            SingleHitbox = true,
            HorizontalAlignment = HorizontalAlignment.Right
        };

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
        public string Password
        {
            get => _Password;
            set
            {
                _Password = value;
                ToggleShowPassword(ShowPassword);

                pwSafetyChart.EvaluatePassword(_Password);
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
            if (KeyHelper.IsKeyPressed(Windows.System.VirtualKey.Control) && !KeyHelper.IsKeyPressed(Windows.System.VirtualKey.Menu))
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
            var sp = GetTemplateChild("pwSafetyChartParentSP") as StackPanel;

            if(!sp.Children.Contains(pwSafetyChart))
                sp.Children.Add(pwSafetyChart);
        }

        public void SetPasswordItems(PasswordItemsManager pwItems)
        {
            pwSafetyChart.SetPasswordItems(pwItems);
        }
    }
}
