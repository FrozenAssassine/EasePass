using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System;
using System.Linq;

namespace EasePass.Controls;

public sealed partial class TextWithSuggestionBox : UserControl
{
    private TextBox _innerTextBox;
    private bool _isUserTextChange;

    public TextWithSuggestionBox()
    {
        InitializeComponent();
    }

    public event SuggestionChosenEvent SuggestionChosen;
    public delegate void SuggestionChosenEvent(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args);

    public event TextChangedEvent TextChanged;
    public delegate void TextChangedEvent(AutoSuggestBox sender, bool isUserTextChange, string text);

    public new event PreviewKeyDownEvent PreviewKeyDown;
    public new delegate void PreviewKeyDownEvent(KeyRoutedEventArgs e);

    public string PlaceholderText
    {
        get => suggestbox.PlaceholderText;
        set => suggestbox.PlaceholderText = value;
    }

    public string Text
    {
        get => _innerTextBox == null ? "" : _innerTextBox.Text;
        set
        {
            _isUserTextChange = false;
            if (_innerTextBox != null)
                _innerTextBox.Text = value;
            else
                suggestbox.Text = value;
        }
    }

    public object SelectedItem { get; private set; }

    public string[] SuggestionItems { get; set; } = null;

    public AutoSuggestBox InternalSuggestBox => suggestbox;

    private void InnerTextBox_PreviewKeyDown(object sender, KeyRoutedEventArgs e)
    {
        if (e.Key == Windows.System.VirtualKey.Down || e.Key == Windows.System.VirtualKey.Up)
        {
            _isUserTextChange = false;
            return;
        }

        _isUserTextChange = true;
    }

    private void InnerTextBox_Paste(object sender, TextControlPasteEventArgs e)
    {
        _isUserTextChange = true;
    }

    private void InnerTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        var text = (sender as TextBox)?.Text;

        TextChanged?.Invoke(suggestbox, _isUserTextChange, text);

        //either make suggestions directly or let the user handle it otherwise
        if (SuggestionItems != null)
            UpdateSuggestions(text, _isUserTextChange);

        _isUserTextChange = false;
    }

    private void suggestbox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
    {
        _isUserTextChange = false;
        SuggestionChosen?.Invoke(sender, args);
    }

    private void UserControl_PreviewKeyDown(object sender, KeyRoutedEventArgs e)
    {
        PreviewKeyDown?.Invoke(e);
    }

    private void TextBox_Loaded(object sender, RoutedEventArgs e)
    {
        _innerTextBox = sender as TextBox;
        _innerTextBox.TextChanged += InnerTextBox_TextChanged;
        _innerTextBox.PreviewKeyDown += InnerTextBox_PreviewKeyDown;
        _innerTextBox.Paste += InnerTextBox_Paste;
    }

    private void SuggestionsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        SelectedItem = (sender as ListView).SelectedItem;
    }

    private void UpdateSuggestions(string? text, bool userChanged)
    {
        if (!userChanged)
            return;

        if (string.IsNullOrWhiteSpace(text))
        {
            suggestbox.ItemsSource = null;
            return;
        }

        var searchKW = text.Trim();
        var suggestions = SuggestionItems.Where(x => x.Contains(searchKW, StringComparison.OrdinalIgnoreCase)).ToList();
        
        if (suggestions.Count == 0)
            suggestions.Add("No results found");

        suggestbox.ItemsSource = suggestions;
    }
}
