using EasePass.Helper;
using EasePass.Models;
using Microsoft.UI.Xaml.Controls;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace EasePass.Views
{
    public sealed partial class PasswordsPage : Page
    {
        private List<PasswordManagerCategory> Items = new List<PasswordManagerCategory>
        {
            new PasswordManagerCategory{
                CategoryName = "Wichtig", IconId = "\xE734",
                Items = new List<PasswordManagerItem>
                {
                    new PasswordManagerItem("github", "\xE731", "1231241234", "MyUserName", "MyEmail@Emai.de", "This are notes\nMore notes"),
                    new PasswordManagerItem("otherhub", "1231241234", "MyUserName2", "MyEmail@Email2.de", "This are notes\nMore notes\and More"),
                    new PasswordManagerItem("thinkhub", "1231241234", "MyUserName5", "MyEmail@Email5.de", "This are notes\nMore notes"),
                }
            },
            new PasswordManagerCategory{
                CategoryName = "Unwichtig", IconId = "\xE7B3",
                Items = new List<PasswordManagerItem>
                {
                    new PasswordManagerItem("Amazon", "\xE7BA", "asldkasldök1", "MyUserNam8e", "MyEmail@Emai12.de", "This are notes\nMore notes"),
                    new PasswordManagerItem("Paypal", "asldkasldök5", "MyUserName9", "MyEmail@Emai5.de", "This are notes\nMore notes\and More"),
                    new PasswordManagerItem("Baguette Bank", "asldkasldök100", "MyUserName6", "MyEmail@Email5.de", "This are notes\nMore notes"),
                }
            }
        };
        private PasswordManagerItem SelectedItem = null;

        public PasswordsPage()
        {
            this.InitializeComponent();

            PasswordItemsManager.PopulateNaivgationView(mainNavigationView, Items);
        }
        private void ShowPasswordItem(PasswordManagerItem item)
        {
            notesTB.Text = item.Notes;
            pwTB.Password = item.Password;
            emailTB.Text = item.Email;
            usernameTB.Text = item.Username;
            itemnameTB.Text = item.DisplayName;
            iconFI.Glyph = item.IconId;
        }


        private void mainNavigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (args.SelectedItem == null)
                return;

            if(args.SelectedItem is NavigationViewItem navItem && navItem.Tag is PasswordManagerItem pwItem)
            {
                SelectedItem = pwItem;
                ShowPasswordItem(pwItem);
            }
        }
        private void passwordSearchBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            // Since selecting an item will also change the text,
            // only listen to changes caused by user entering text.
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                var suitableItems = PasswordItemsManager.FindItemsByName(Items, sender.Text.ToLower());
                if (suitableItems.Count == 0)
                    return; //TODO Message nothing found:

                sender.ItemsSource = suitableItems;
            }
        }
        private void passwordSearchBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            
        }
    }
}
