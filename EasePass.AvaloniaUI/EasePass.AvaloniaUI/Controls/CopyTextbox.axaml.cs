using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using CommunityToolkit.Mvvm.Input;
using EasePass.Helper;
using System;
using System.Diagnostics;

namespace EasePass.Controls
{
    public partial class CopyTextbox : UserControl
    {
        public CopyTextbox()
        {
            InitializeComponent();

            rootTB = this.FindControl<TextBox>("rootTB");
            rootTB.AddHandler(PointerPressedEvent, TextBox_PointerPressed, RoutingStrategies.Tunnel | RoutingStrategies.Bubble, true);
        }

        public static readonly StyledProperty<bool> UseRevealClassProperty =
            AvaloniaProperty.Register<CopyTextbox, bool>(nameof(UseRevealClass));

        public bool UseRevealClass
        {
            get => GetValue(UseRevealClassProperty);
            set => SetValue(UseRevealClassProperty, value);
        }

        public static readonly StyledProperty<char> PasswordCharProperty =
    AvaloniaProperty.Register<CopyTextbox, char>(nameof(PasswordChar));

        public char PasswordChar
        {
            get => GetValue(PasswordCharProperty);
            set => SetValue(PasswordCharProperty, value);
        }

        public static readonly StyledProperty<string> TextProperty =
            AvaloniaProperty.Register<CopyTextbox, string>(nameof(Text));

        public string Text
        {
            get => GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public static readonly StyledProperty<string> HeaderProperty =
            AvaloniaProperty.Register<CopyTextbox, string>(nameof(Header));

        public string Header
        {
            get => GetValue(HeaderProperty);
            set => SetValue(HeaderProperty, value);
        }

        public static readonly StyledProperty<bool> IsReadOnlyProperty =
            AvaloniaProperty.Register<CopyTextbox, bool>(nameof(IsReadOnly));

        public bool IsReadOnly
        {
            get => GetValue(IsReadOnlyProperty);
            set => SetValue(IsReadOnlyProperty, value);
        }

        public static readonly StyledProperty<string> WatermarkProperty =
            AvaloniaProperty.Register<CopyTextbox, string>(nameof(Watermark));

        public string Watermark
        {
            get => GetValue(WatermarkProperty);
            set => SetValue(WatermarkProperty, value);
        }

        public static readonly StyledProperty<bool> AcceptsReturnProperty =
            AvaloniaProperty.Register<CopyTextbox, bool>(nameof(AcceptsReturn));

        public bool AcceptsReturn
        {
            get => GetValue(AcceptsReturnProperty);
            set => SetValue(AcceptsReturnProperty, value);
        }

        public bool RemoveWhitespaceOnCopy { get; set; } = false;
        public bool IsUrlAction { get; set; } = false;

        private async void CopyText_Click(object sender, RoutedEventArgs e)
        {
            string txt = this.Text;
            if (string.IsNullOrEmpty(txt))
                return;

            if (IsUrlAction)
            {
                // Simplified URL launch logic
                if (!txt.ToLower().StartsWith("http")) txt = "http://" + txt;
                try
                {
                    // Avalonia doesn't have direct Launcher.LaunchUriAsync in core, usually needs platform specific or Process.Start
                    // For now, I'll use a helper if available or standard Process start.
                    // Assuming RequestsHelper or similar might have it, or simple Process.Start wrapper.
                    // For compiling safety, I'll use ClipboardHelper for copy as fallback or just implemented check.

                    var launcher = TopLevel.GetTopLevel(this)?.Launcher;
                    if (launcher != null)
                    {
                        await launcher.LaunchUriAsync(new Uri(txt));
                        return;
                    }
                }
                catch { /*Invalid URL*/ return; }
            }

            await ClipboardHelper.CopyAsync(RemoveWhitespaceOnCopy ? this.Text.Replace(" ", "") : this.Text);
        }

        private void TextBox_PointerPressed(object? sender, PointerPressedEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                Debug.WriteLine("double clicked");
                CopyText_Click(null, null);
            }
        }
    }
}
