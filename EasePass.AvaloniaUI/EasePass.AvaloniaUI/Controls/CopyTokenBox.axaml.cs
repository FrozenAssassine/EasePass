using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using EasePass.Helper;
using System;

namespace EasePass.Controls
{
    public partial class CopyTokenBox : UserControl
    {
        public CopyTokenBox()
        {
            InitializeComponent();
        }

        public static readonly StyledProperty<string> TokenProperty =
            AvaloniaProperty.Register<CopyTokenBox, string>(nameof(Token));

        public string Token
        {
            get => GetValue(TokenProperty);
            set => SetValue(TokenProperty, value);
        }

        public static readonly StyledProperty<bool> ShowTokenProperty =
            AvaloniaProperty.Register<CopyTokenBox, bool>(nameof(ShowToken));

        public bool ShowToken
        {
            get => GetValue(ShowTokenProperty);
            set => SetValue(ShowTokenProperty, value);
        }
        
        public static readonly StyledProperty<string> HeaderProperty =
            AvaloniaProperty.Register<CopyTokenBox, string>(nameof(Header));

        public string Header
        {
            get => GetValue(HeaderProperty);
            set => SetValue(HeaderProperty, value);
        }
        
         public static readonly StyledProperty<string> WatermarkProperty =
            AvaloniaProperty.Register<CopyTokenBox, string>(nameof(Watermark));

        public string Watermark
        {
            get => GetValue(WatermarkProperty);
            set => SetValue(WatermarkProperty, value);
        }

        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);

            if (change.Property == TokenProperty || change.Property == ShowTokenProperty)
            {
                UpdateText();
            }
        }

        private void UpdateText()
        {
            var tb = this.FindControl<TextBox>("textBox");
            if (tb != null)
            {
                tb.Text = Token;
                tb.PasswordChar = ShowToken ? default(char) : '•';
            }
        }

        private async void CopyText_Click(object sender, RoutedEventArgs e)
        {
            await ClipboardHelper.CopyAsync(this.Token, true);
        }
    }
}
