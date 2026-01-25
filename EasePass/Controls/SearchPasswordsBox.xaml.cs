using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;

namespace EasePass.Controls
{
    //a reason, why I not directly use the SuggestionChosenEvent from the suggestbox, 
    //is because the event is way slower, so the search feels more laggy and slow.
    //thats why this code is more complex then it could be.
    public sealed partial class SearchPasswordsBox : UserControl
    {
        private TextBlock _infoLabel;
        private TextBox _innerTextBox;
        private bool _isUserTextChange;

        public SearchPasswordsBox()
        {
            InitializeComponent();
        }

        public event SuggestionChosenEvent SuggestionChosen;
        public delegate void SuggestionChosenEvent(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args);

        public event TextChangedEvent TextChanged;
        public delegate void TextChangedEvent(AutoSuggestBox sender, bool isUserTextChange, string text);

        public new event PreviewKeyDownEvent PreviewKeyDown;
        public new delegate void PreviewKeyDownEvent(bool isTagSearch, KeyRoutedEventArgs e);


        public string PlaceholderText
        {
            get => suggestbox.PlaceholderText;
            set => suggestbox.PlaceholderText = value;
        }

        public string Text
        {
            get => _innerTextBox.Text;
            set
            {
                _isUserTextChange = false;
                _innerTextBox.Text = value;
            }
        }



        public object SelectedItem { get; private set; }

        public AutoSuggestBox InternalSuggestBox => suggestbox;

        public string InfoLabel
        {
            get => _infoLabel?.Text;
            set
            {
                if (_infoLabel != null)
                    _infoLabel.Text = value;
            }
        }

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
            _isUserTextChange = false;
        }

        private void suggestbox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            _isUserTextChange = false;
            SuggestionChosen?.Invoke(sender, args);
        }

        private void infoLabel_Loaded(object sender, RoutedEventArgs e)
        {
            _infoLabel = sender as TextBlock;
        }

        private void UserControl_PreviewKeyDown(object sender, KeyRoutedEventArgs e)
        {
            PreviewKeyDown?.Invoke(Text?.StartsWith("/") == true, e);
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
    }
}
