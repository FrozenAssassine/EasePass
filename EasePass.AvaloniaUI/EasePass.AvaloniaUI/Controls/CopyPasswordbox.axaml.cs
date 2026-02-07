using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using EasePass.Helper;
using System;

namespace EasePass.Controls
{
    public partial class CopyPasswordbox : UserControl
    {
        public CopyPasswordbox()
        {
            InitializeComponent();
        }

        public static readonly StyledProperty<string> PasswordProperty =
            AvaloniaProperty.Register<CopyPasswordbox, string>(nameof(Password));

        public string Password
        {
            get => GetValue(PasswordProperty);
            set => SetValue(PasswordProperty, value);
        }

        public static readonly StyledProperty<bool> ShowPasswordProperty =
            AvaloniaProperty.Register<CopyPasswordbox, bool>(nameof(ShowPassword));

        public bool ShowPassword
        {
            get => GetValue(ShowPasswordProperty);
            set => SetValue(ShowPasswordProperty, value);
        }
        
        public static readonly StyledProperty<string> HeaderProperty =
            AvaloniaProperty.Register<CopyPasswordbox, string>(nameof(Header));

        public string Header
        {
            get => GetValue(HeaderProperty);
            set => SetValue(HeaderProperty, value);
        }

        public static readonly StyledProperty<string> WatermarkProperty =
            AvaloniaProperty.Register<CopyPasswordbox, string>(nameof(Watermark));

        public string Watermark
        {
            get => GetValue(WatermarkProperty);
            set => SetValue(WatermarkProperty, value);
        }

        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);

            if (change.Property == PasswordProperty)
            {
                UpdateText();
                var chart = this.FindControl<PasswordSafetyChart>("pwSafetyChart");
                if (chart != null && Password != null)
                {
                    chart.EvaluatePassword(Password, true);
                }
            }
            else if (change.Property == ShowPasswordProperty)
            {
                UpdateText();
            }
        }

        private void UpdateText()
        {
            var tb = this.FindControl<TextBox>("textBox");
            if (tb != null)
            {
                tb.Text = Password;
                tb.PasswordChar = ShowPassword ? default(char) : '•';
                // Note: With PasswordChar set, TextBox displays dots. 
                // But ReadOnly TextBox might still support PasswordChar.
                // If not, we fall back to manual text replacement:
                // tb.Text = ShowPassword ? Password : new string('•', Password?.Length ?? 0);
            }
        }

        private async void CopyText_Click(object sender, RoutedEventArgs e)
        {
            await ClipboardHelper.CopyAsync(this.Password, true);
        }
    }
}
