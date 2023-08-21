using EasePass.Dialogs;
using EasePass.Extensions;
using EasePass.Helper;
using EasePass.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security;
using System.Threading.Tasks;

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
        private async Task DeletePasswordItem(PasswordManagerItem item)
        {
            if (item == null)
                return;

            if (await new DeleteConfirmationDialog().ShowAsync(item))
            {
                int index = passwordItemListView.SelectedIndex;
                PasswordItemsManager.DeleteItem(PasswordItems, item);

                if (passwordItemListView.Items.Count >= 1)
                    passwordItemListView.SelectedIndex = index - 1 > 0 ? index - 1 : index + 1 < passwordItemListView.Items.Count ? index + 1 : 0;
                else
                    passwordShowArea.Visibility = Visibility.Collapsed;

                //update searchbox:
                if (searchbox.Text.Length > 0)
                {
                    ObservableCollection<PasswordManagerItem> items = PasswordItemsManager.FindItemsByName(PasswordItems, searchbox.Text);
                    passwordItemListView.ItemsSource = items;
                    searchbox.SetInfo(items.Count.ToString());
                }

                SaveData();
            }
        }
        private async Task EditPasswordItem(PasswordManagerItem item)
        {
            if (item == null)
                return;

            var editItem = await new EditItemDialog().ShowAsync(item);
            if (editItem == null)
                return;

            ShowPasswordItem(SelectedItem);
            SaveData();
        }
        private async Task AddPasswordItem()
        {
            var newItem = await new AddItemDialog().ShowAsync();
            if (newItem == null)
                return;

            PasswordItemsManager.AddItem(PasswordItems, newItem);
            SaveData();
        }
        private async Task Add2FAPasswordItem(PasswordManagerItem item)
        {
            if (item == null)
                return;

            if (!await new Add2FADialog().ShowAsync(item))
                return;

            ShowPasswordItem(item);
            SaveData();
        }
        private async Task GeneratePassword()
        {
            //returns true when the regenerate button was pressed
            var res = await new GenPasswordDialog().ShowAsync();
            if (res)
                await GeneratePassword();
        }
        private void ShowSettingsPage()
        {
            App.m_frame.Navigate(typeof(SettingsPage), new SettingsNavigationParameters
            {
                PasswordPage = this,
                PwItems = PasswordItems
            });
        }

        private async void DeletePasswordItem_Click(object sender, RoutedEventArgs e) => await DeletePasswordItem(SelectedItem);
        private async void AddPasswordItem_Click(object sender, RoutedEventArgs e) => await AddPasswordItem();
        private async void EditPasswordItem_Click(object sender, RoutedEventArgs e) => await EditPasswordItem(SelectedItem);
        private async void Add2FAPasswordItem_Click(object sender, RoutedEventArgs e) => await Add2FAPasswordItem(SelectedItem);
        private async void GenPassword_Click(object sender, RoutedEventArgs e) => await GeneratePassword();
        private void Settings_Click(object sender, RoutedEventArgs e) => ShowSettingsPage();

        private void PasswordItemListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (passwordItemListView.Items.Count == 0)
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
        private void PasswordItemListView_DragItemsCompleted(ListViewBase sender, DragItemsCompletedEventArgs args)
        {
            if (args.Items.Count > 0)
                SaveData();
        }


        private void Searchbox_TextChanged(object sender, TextChangedEventArgs e)
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

        private void Searchbox_PreviewKeyDown(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
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


        private void SortName_Click(object sender, RoutedEventArgs e)
        {
            PasswordItems.Sort(ObservableCollectionExtension.ByDisplayName);
            Reload();
            SaveData();
            SetVis(sortname);
        }
        private void SortUsername_Click(object sender, RoutedEventArgs e)
        {
            PasswordItems.Sort(ObservableCollectionExtension.ByUsername);
            Reload();
            SaveData();
            SetVis(sortusername);
        }
        private void SortNotes_Click(object sender, RoutedEventArgs e)
        {
            PasswordItems.Sort(ObservableCollectionExtension.ByNotes);
            Reload();
            SaveData();
            SetVis(sortnotes);
        }
        private void SortWebsite_Click(object sender, RoutedEventArgs e)
        {
            PasswordItems.Sort(ObservableCollectionExtension.ByWebsite);
            Reload();
            SaveData();
            SetVis(sortwebsite);
        }
        private void SwitchOrder_Click(object sender, RoutedEventArgs e)
        {
            PasswordItems = PasswordItems.ReverseSelf();
            Reload();
            SaveData();
        }


        private void RightclickedItem_CopyUsername_Click(object sender, RoutedEventArgs e) => ClipboardHelper.Copy(((sender as MenuFlyoutItem)?.Tag as PasswordManagerItem)?.Username);
        private void RightclickedItem_CopyEmail_Click(object sender, RoutedEventArgs e) => ClipboardHelper.Copy(((sender as MenuFlyoutItem)?.Tag as PasswordManagerItem)?.Email);
        private void RightclickedItem_CopyPassword_Click(object sender, RoutedEventArgs e) => ClipboardHelper.Copy(((sender as MenuFlyoutItem)?.Tag as PasswordManagerItem)?.Password);
        private async void RightclickedItem_Delete_Click(object sender, RoutedEventArgs e) => await DeletePasswordItem((sender as MenuFlyoutItem)?.Tag as PasswordManagerItem);
        private async void RightclickedItem_Edit_Click(object sender, RoutedEventArgs e) => await EditPasswordItem((sender as MenuFlyoutItem)?.Tag as PasswordManagerItem);

        private void SetVis(FontIcon icon)
        {
            sortname.Visibility = icon == sortname ? Visibility.Visible : Visibility.Collapsed;
            sortusername.Visibility = icon == sortusername ? Visibility.Visible : Visibility.Collapsed;
            sortnotes.Visibility = icon == sortnotes ? Visibility.Visible : Visibility.Collapsed;
            sortwebsite.Visibility = icon == sortwebsite ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
