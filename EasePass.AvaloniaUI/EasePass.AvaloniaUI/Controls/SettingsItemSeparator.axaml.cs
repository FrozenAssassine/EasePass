using Avalonia;
using Avalonia.Controls;
using Avalonia.Metadata;

namespace EasePass.AvaloniaUI.Controls
{
    public partial class SettingsItemSeparator : UserControl
    {
        public SettingsItemSeparator()
        {
            InitializeComponent();
        }

        public static readonly StyledProperty<string> HeaderProperty =
            AvaloniaProperty.Register<SettingsItemSeparator, string>(nameof(Header));

        public string Header
        {
            get => GetValue(HeaderProperty);
            set => SetValue(HeaderProperty, value);
        }

        public static readonly StyledProperty<object> SettingsContentProperty =
            AvaloniaProperty.Register<SettingsItemSeparator, object>(nameof(SettingsContent));

        [Content]
        public object SettingsContent
        {
            get => GetValue(SettingsContentProperty);
            set => SetValue(SettingsContentProperty, value);
        }
    }
}
