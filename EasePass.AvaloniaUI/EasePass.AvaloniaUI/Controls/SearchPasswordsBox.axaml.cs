using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using System;

namespace EasePass.Controls
{
    public partial class SearchPasswordsBox : UserControl
    {
        private bool _isUserTextChange;

        public SearchPasswordsBox()
        {
            InitializeComponent();
            var box = this.FindControl<AutoCompleteBox>("suggestbox");
            if (box != null)
            {
                box.TextChanged += Box_TextChanged;
                box.SelectionChanged += Box_SelectionChanged;
            }
        }

        public delegate void TextChangedEvent(object sender, bool isUserTextChange, string text);
        public event TextChangedEvent TextChanged;

        public delegate void SuggestionChosenEvent(object sender, SelectionChangedEventArgs args);
        public event SuggestionChosenEvent SuggestionChosen;

        public new delegate void PreviewKeyDownEvent(bool isTagSearch, KeyEventArgs e);
        public new event PreviewKeyDownEvent PreviewKeyDown;

        public static readonly StyledProperty<string> PlaceholderTextProperty =
            AvaloniaProperty.Register<SearchPasswordsBox, string>(nameof(PlaceholderText));

        public string PlaceholderText
        {
            get => GetValue(PlaceholderTextProperty);
            set => SetValue(PlaceholderTextProperty, value);
        }

        public static readonly StyledProperty<string> InfoLabelProperty =
            AvaloniaProperty.Register<SearchPasswordsBox, string>(nameof(InfoLabel));

        public string InfoLabel
        {
            get => GetValue(InfoLabelProperty);
            set => SetValue(InfoLabelProperty, value);
        }

        public string Text
        {
            get => this.FindControl<AutoCompleteBox>("suggestbox")?.Text ?? "";
            set
            {
                var box = this.FindControl<AutoCompleteBox>("suggestbox");
                if (box != null)
                {
                    _isUserTextChange = false;
                    box.Text = value;
                }
            }
        }

        public AutoCompleteBox InternalSuggestBox => this.FindControl<AutoCompleteBox>("suggestbox");
        public object SelectedItem { get; private set; }

        private void Box_TextChanged(object sender, EventArgs e)
        {
            var box = sender as AutoCompleteBox;
            
            // Assume user text change if not explicitly suppressed
            if (!_isUserTextChange)
                 _isUserTextChange = true; 

            var text = box?.Text;
            TextChanged?.Invoke(box, _isUserTextChange, text);

            // Logic reset
            _isUserTextChange = true;
        }

        private void Box_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _isUserTextChange = false;
            SelectedItem = (sender as AutoCompleteBox)?.SelectedItem;
            SuggestionChosen?.Invoke(sender, e);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
             // Avalonia KeyDown event bubble/tunnel.
             // We can use PreviewKeyDown equivalent which is AddHandler(KeyDownEvent, RoutingStrategies.Tunnel)
             // But here we expose an event. 
             bool isTagSearch = Text?.StartsWith("/") == true;
             PreviewKeyDown?.Invoke(isTagSearch, e);
             
             base.OnKeyDown(e);
        }
    }
}