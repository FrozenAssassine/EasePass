using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Metadata;

namespace EasePass.Controls
{
    public partial class SettingsControl : UserControl
    {
        public delegate void ClickedEvent(SettingsControl sender);
        public event ClickedEvent Clicked;

        public SettingsControl()
        {
            InitializeComponent();
        }

        public static readonly StyledProperty<bool> ClickableProperty =
            AvaloniaProperty.Register<SettingsControl, bool>(nameof(Clickable));

        public bool Clickable
        {
            get => GetValue(ClickableProperty);
            set 
            {
                SetValue(ClickableProperty, value);
                var grid = this.FindControl<Grid>("mainGrid");
                if (grid != null) grid.Tag = value; // Proxy for selector
            }
        }

        public static readonly StyledProperty<string> GlyphProperty =
            AvaloniaProperty.Register<SettingsControl, string>(nameof(Glyph));

        public string Glyph
        {
            get => GetValue(GlyphProperty);
            set => SetValue(GlyphProperty, value);
        }

        public static readonly StyledProperty<string> HeaderProperty =
            AvaloniaProperty.Register<SettingsControl, string>(nameof(Header));

        public string Header
        {
            get => GetValue(HeaderProperty);
            set => SetValue(HeaderProperty, value);
        }

        public static readonly StyledProperty<object> SettingsContentProperty =
            AvaloniaProperty.Register<SettingsControl, object>(nameof(SettingsContent));

        [Content]
        public object SettingsContent
        {
            get => GetValue(SettingsContentProperty);
            set => SetValue(SettingsContentProperty, value);
        }
        
        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);
            if (change.Property == ClickableProperty)
            {
                var grid = this.FindControl<Grid>("mainGrid");
                if (grid != null) grid.Tag = change.NewValue;
            }
        }

        private void mainGrid_PointerPressed(object sender, PointerPressedEventArgs e)
        {
            if (Clickable)
                 Clicked?.Invoke(this);
        }
    }
}
