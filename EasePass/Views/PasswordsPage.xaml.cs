using EasePass.Dialogs;
using EasePass.Helper;
using EasePass.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System.Collections.ObjectModel;
using System.Security;

namespace EasePass.Views
{
    public sealed partial class PasswordsPage : Page
    {
        private ObservableCollection<PasswordManagerItem> PasswordItems = new ObservableCollection<PasswordManagerItem>();
        private PasswordManagerItem SelectedItem = null;
        private SecureString masterPassword = null;
        private FrameworkElement rightClickedItem = null;

        public PasswordsPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            App.m_window.ShowBackArrow = false;
            //when navigating from settings:
            if (e.Parameter == null)
            {
                passwordItemListView.ItemsSource = PasswordItems;
                SaveData();
                return;
            }

            if(e.Parameter is SecureString pw)
            {
                masterPassword = pw;
                LoadData(masterPassword);
            }

            base.OnNavigatedTo(e);
        }

        private void LoadData(SecureString pw)
        {
            var data = DatabaseHelper.LoadDatabase(pw);
            if (data == null)
                return;

            PasswordItems = data;
        }
        private void SaveData()
        {
            DatabaseHelper.SaveDatabase(PasswordItems, masterPassword);
        }

        private void ShowPasswordItem(PasswordManagerItem item)
        {
            notesTB.Text = item.Notes;
            pwTB.Text = item.Password;
            emailTB.Text = item.Email;
            usernameTB.Text = item.Username;
            itemnameTB.Text = item.DisplayName;

            passwordShowArea.Visibility = Visibility.Visible;
        }


        private void passwordItemListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (passwordItemListView.SelectedItem == null)
            {
                SelectedItem = null;
                return;
            }

            if (passwordItemListView.SelectedItem is PasswordManagerItem pwItem)
            {
                SelectedItem = pwItem;
                ShowPasswordItem(pwItem);
            }
        }
        private async void DeletePasswordItem_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedItem == null)
                return;

            if (await new DeleteConfirmationDialog().ShowAsync(SelectedItem))
            {
                int index = passwordItemListView.SelectedIndex;
                PasswordItemsManager.DeleteItem(PasswordItems, SelectedItem);

                if (PasswordItems.Count >= 1)
                    passwordItemListView.SelectedIndex = index - 1 > 0 ? index - 1 :
                        index + 1 < PasswordItems.Count ? index + 1 : 0;
                else
                    passwordShowArea.Visibility = Visibility.Collapsed;

                SaveData();
            }
        }
        private async void AddPasswordItem_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            var newItem = await new AddItemDialog().ShowAsync(PasswordItems);
            if (newItem == null)
                return;

            PasswordItemsManager.AddItem(PasswordItems, newItem);
            SaveData();
        }
        private async void EditPasswordItem_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedItem == null)
                return;

            var editItem = await new EditItemDialog().ShowAsync(PasswordItems, SelectedItem);
            if (editItem == null)
                return;

            ShowPasswordItem(SelectedItem);
            SaveData();
        }
        private void searchbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(searchbox.Text == "")
            {
                passwordItemListView.ItemsSource = PasswordItems;
                return;
            }
            passwordItemListView.ItemsSource = PasswordItemsManager.FindItemsByName(PasswordItems, searchbox.Text);
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            App.m_frame.Navigate(typeof(SettingsPage), PasswordItems);
        }
        private void AboutPage_Click(object sender, RoutedEventArgs e)
        {
            App.m_frame.Navigate(typeof(AboutPage));
        }
    }
}
