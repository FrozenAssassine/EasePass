using EasePass.Dialogs;
using EasePass.Helper;
using EasePass.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security;

namespace EasePass.Views
{
    public sealed partial class PasswordsPage : Page
    {
        private ObservableCollection<PasswordManagerItem> PasswordItems = new ObservableCollection<PasswordManagerItem>();
        private PasswordManagerItem SelectedItem = null;
        public SecureString masterPassword = null;
        private AutoBackupHelper autoBackupHelper = new AutoBackupHelper();
        public const int TOTP_SPACING = 3;
        private TOTPTokenUpdater totpTokenUpdater;

        public PasswordsPage()
        {
            this.InitializeComponent();

            autoBackupHelper.Start(this, PasswordItems);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            App.m_window.ShowBackArrow = false;

            if (e.NavigationMode == NavigationMode.Back)
            {
                passwordItemListView.ItemsSource = PasswordItems;
                SaveData();
                autoBackupHelper.UpdateSettings();
            }
            else if(e.NavigationMode == NavigationMode.New && e.Parameter is SecureString pw)
            {
                masterPassword = pw;
                LoadData(masterPassword);
            }
   
            base.OnNavigatedTo(e);
        }

        public void Reload()
        {
            passwordItemListView.ItemsSource = null;
            passwordItemListView.ItemsSource = PasswordItems;
        }

        public void Sort()
        {
            Sort(PasswordItems, (PasswordManagerItem pmi1, PasswordManagerItem pmi2) =>
            {
                return pmi1.DisplayName.CompareTo(pmi2.DisplayName);
            });
        }

        public static void Sort<T>(ObservableCollection<T> collection, Comparison<T> comparison)
        {
            var sortableList = new List<T>(collection);
            sortableList.Sort(comparison);

            for (int i = 0; i < sortableList.Count; i++)
            {
                collection.Move(collection.IndexOf(sortableList[i]), i);
            }
        }

        private void LoadData(SecureString pw)
        {
            var data = DatabaseHelper.LoadDatabase(pw);
            if (data == null)
                return;

            PasswordItems = data;
        }

        public void SaveData()
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
            websiteTB.Text = item.Website;

            passwordShowArea.Visibility = Visibility.Visible;

            if (!string.IsNullOrEmpty(item.Secret))
            {
                totpTB.Visibility = totpLB.Visibility = Visibility.Visible;
                totpTokenUpdater = new TOTPTokenUpdater(item, totpTB);
                totpTokenUpdater.StartTimer();
                totpTokenUpdater.SimulateTickEvent();
                return;
            }

            totpTB.Visibility = totpLB.Visibility = Visibility.Collapsed;
            if(totpTokenUpdater != null)
                totpTokenUpdater.StopTimer();
        }

        private void passwordItemListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(passwordItemListView.Items.Count == 0)
            {
                passwordShowArea.Visibility = Visibility.Collapsed;
            }

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

                if (passwordItemListView.Items.Count >= 1)
                    passwordItemListView.SelectedIndex = index - 1 > 0 ? index - 1 :
                        index + 1 < passwordItemListView.Items.Count ? index + 1 : 0;
                else
                    passwordShowArea.Visibility = Visibility.Collapsed;

                //update searchbox:
                if(searchbox.Text.Length > 0)
                    passwordItemListView.ItemsSource = PasswordItemsManager.FindItemsByName(PasswordItems, searchbox.Text);
                
                SaveData();
            }
        }

        private async void AddPasswordItem_Click(object sender, RoutedEventArgs e)
        {
            var newItem = await new AddItemDialog().ShowAsync();
            if (newItem == null)
                return;

            PasswordItemsManager.AddItem(PasswordItems, newItem);
            SaveData();
        }

        private async void EditPasswordItem_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedItem == null)
                return;

            var editItem = await new EditItemDialog().ShowAsync(SelectedItem);
            if (editItem == null)
                return;

            ShowPasswordItem(SelectedItem);
            SaveData();
        }

        private void searchbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(searchbox.Text.Length == 0)
            {
                passwordItemListView.ItemsSource = PasswordItems;
                searchbox.SetInfo(Convert.ToString(PasswordItems.Count));
                return;
            }
            var search_res = PasswordItemsManager.FindItemsByName(PasswordItems, searchbox.Text);
            passwordItemListView.ItemsSource = search_res;
            searchbox.SetInfo(Convert.ToString(search_res.Count));
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            App.m_frame.Navigate(typeof(SettingsPage), new SettingsNavigationParameters
            {
                PasswordPage = this,
                PwItems = PasswordItems
            });
        }

        private void AboutPage_Click(object sender, RoutedEventArgs e)
        {
            App.m_frame.Navigate(typeof(AboutPage));
        }

        private void Page_KeyDown(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (KeyHelper.IsKeyPressed(Windows.System.VirtualKey.Control))
            {
                switch (e.Key)
                {
                    case Windows.System.VirtualKey.F:
                        searchbox.Focus(FocusState.Programmatic);
                        break;
                    case Windows.System.VirtualKey.N:
                        AddPasswordItem_Click(null, null);
                        break;
                    case Windows.System.VirtualKey.E:
                        EditPasswordItem_Click(null, null);
                        break;
                    case Windows.System.VirtualKey.Down:
                        if (passwordItemListView.SelectedIndex >= 0 && passwordItemListView.SelectedIndex < passwordItemListView.Items.Count - 1)
                            passwordItemListView.SelectedIndex++;
                        break;
                    case Windows.System.VirtualKey.Up:
                        if(passwordItemListView.SelectedIndex > 0 && passwordItemListView.SelectedIndex < passwordItemListView.Items.Count)
                            passwordItemListView.SelectedIndex--;
                        break;
                    default: return;
                }
            }

            switch (e.Key)
            {
                case Windows.System.VirtualKey.F1:
                    Settings_Click(null, null);
                    break;
                case Windows.System.VirtualKey.F2:
                    EditPasswordItem_Click(null, null);
                    break;
            }
        }

        private async void Add2FAPasswordItem_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedItem == null)
                return;

            if (!await new Add2FADialog().ShowAsync(SelectedItem))
                return;

            ShowPasswordItem(SelectedItem);
            SaveData();
        }

        private async void GenPassword_Click(object sender, RoutedEventArgs e)
        {
            //retunrs true when the regenerate button was pressed
            var res = await new GenPasswordDialog().ShowAsync();
            if (res)
                GenPassword_Click(this, null);
        }
        private void searchbox_PreviewKeyDown(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Down)
            {
                passwordItemListView.Focus(FocusState.Programmatic);
                passwordItemListView.SelectedIndex = -1;
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            searchbox.SetInfo(Convert.ToString(PasswordItems.Count));
        }

        private void sortAlphabetical_Click(object sender, RoutedEventArgs e)
        {
            Sort();
            Reload();
            SaveData();
        }
    }
}
