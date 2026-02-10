using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using EasePass.Helper;
using System;
using System.Diagnostics;

namespace EasePass.Controls
{
    public partial class CopyPasswordbox : UserControl
    {
        public CopyPasswordbox()
        {
            InitializeComponent();

            rootTB = this.FindControl<TextBox>("rootTB");
            rootTB.AddHandler(PointerPressedEvent, TextBox_PointerPressed, RoutingStrategies.Tunnel | RoutingStrategies.Bubble, true);
        }

        public static readonly StyledProperty<string> PasswordProperty =
            AvaloniaProperty.Register<CopyPasswordbox, string>(nameof(Password));

        public string Password
        {
            get => GetValue(PasswordProperty);
            set => SetValue(PasswordProperty, value);
        }

        public static readonly StyledProperty<string> WatermarkProperty =
            AvaloniaProperty.Register<CopyPasswordbox, string>(nameof(Watermark));

        public string Watermark
        {
            get => GetValue(WatermarkProperty);
            set => SetValue(WatermarkProperty, value);
        }

        private async void CopyText_Click(object sender, RoutedEventArgs e)
        {
            await ClipboardHelper.CopyAsync(this.Password, true);
        }
        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);

            if (change.Property == PasswordProperty)
            {
                var chart = this.FindControl<PasswordSafetyChart>("pwSafetyChart");
                if (chart != null && Password != null)
                {
                    chart.EvaluatePassword(Password, true);
                }
            }
        }
        private void TextBox_PointerPressed(object? sender, PointerPressedEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                CopyText_Click(null, null);
            }
        }
    }
}
