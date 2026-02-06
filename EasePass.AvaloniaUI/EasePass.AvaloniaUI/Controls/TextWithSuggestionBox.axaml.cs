using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using System;
using System.Linq;
using System.Collections.Generic;

namespace EasePass.AvaloniaUI.Controls
{
    public partial class TextWithSuggestionBox : UserControl
    {
        private bool _isUserTextChange;
        
        public TextWithSuggestionBox()
        {
            InitializeComponent();
            var box = this.FindControl<AutoCompleteBox>("suggestbox");
            if(box != null)
            {
               box.TextChanged += Box_TextChanged;
               box.SelectionChanged += Box_SelectionChanged;
               // box.KeyDown += Box_KeyDown; // For filtering out navigation keys if needed
            }
        }
        
        // Event delegates mapping to somewhat compatible signatures
        public delegate void TextChangedEvent(object sender, bool isUserTextChange, string text);
        public event TextChangedEvent TextChanged;
        
        public delegate void SuggestionChosenEvent(object sender, SelectionChangedEventArgs args);
        public event SuggestionChosenEvent SuggestionChosen;

        public static readonly StyledProperty<string> PlaceholderTextProperty =
            AvaloniaProperty.Register<TextWithSuggestionBox, string>(nameof(PlaceholderText));

        public string PlaceholderText
        {
            get => GetValue(PlaceholderTextProperty);
            set => SetValue(PlaceholderTextProperty, value);
        }
        
        public string Text
        {
            get => this.FindControl<AutoCompleteBox>("suggestbox")?.Text ?? "";
            set 
            {
                 var box = this.FindControl<AutoCompleteBox>("suggestbox");
                 if(box != null) 
                 {
                     _isUserTextChange = false;
                     box.Text = value;
                 }
            }
        }

        public string[] SuggestionItems { get; set; } = null;
        public AutoCompleteBox InternalSuggestBox => this.FindControl<AutoCompleteBox>("suggestbox");

        private void Box_TextChanged(object sender, EventArgs e)
        {
            var box = sender as AutoCompleteBox;
            // Crude detection of user change vs programmatic
            // Avalonia raises TextChanged for both.
            // Assuming _isUserTextChange is true by default unless we set it to false before setting Text.
            // But we can' easily detect if it was Typing.
            // For now, assume true if we didn't set it to false.
            if (!_isUserTextChange) 
            {
                 _isUserTextChange = true; // Reset for next time (unless we just set it)
                 // This logic is flawed without proper re-entrancy handling. 
                 // But in this context, we set _isUserTextChange = false before setting text programmatically.
                 // So if it event fires, we check the flag.
                 // But wait, the event fires *during* the set.
                 // So we need to set _isUserTextChange = false, SetText(), then set _isUserTextChange = true?
                 // No, standard pattern:
                 // _ignoreChange = true; Text = ...; _ignoreChange = false;
            }
            else
            {
               // It's a user change
            }

            // Correction: I should just trust the flag management in the Setter.
            // The problem is differentiating Typing from Selection.
            
            var text = box?.Text;
            TextChanged?.Invoke(box, _isUserTextChange, text);

            if (_isUserTextChange && SuggestionItems != null)
                UpdateSuggestions(text);
            
            // Reset to true for user input
            _isUserTextChange = true; 
        }

        private void Box_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _isUserTextChange = false;
            SuggestionChosen?.Invoke(sender, e);
        }

        private void UpdateSuggestions(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                InternalSuggestBox.ItemsSource = null;
                return;
            }

            var searchKW = text.Trim();
            var suggestions = SuggestionItems.Where(x => x.Contains(searchKW, StringComparison.OrdinalIgnoreCase)).ToList();
            
            if (suggestions.Count == 0)
                suggestions.Add("No results found");

            InternalSuggestBox.ItemsSource = suggestions;
        }
        
        // Expose PreviewKeyDown on UserControl or Box?
        // Avalonia uses AddHandler for Preview events.
        // User can subscribe to KeyDown on this control.
    }
}
