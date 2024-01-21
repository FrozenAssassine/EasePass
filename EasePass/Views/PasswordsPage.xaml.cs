using EasePass.Core;
using EasePass.Dialogs;
using EasePass.Extensions;
using EasePass.Helper;
using EasePass.Models;
using EasePass.Settings;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Runtime.Intrinsics.X86;
using System.Security;
using System.Threading.Tasks;
using System.Timers;

namespace EasePass.Views
{
    public sealed partial class PasswordsPage : Page
    {
        public delegate int PasswordExists(string password);
        public SecureString masterPassword = null;
        public const int TOTP_SPACING = 3;
        private PasswordManagerItem SelectedItem = null;
        private TOTPTokenUpdater totpTokenUpdater;
        public readonly PasswordItemsManager passwordItemsManager = new PasswordItemsManager();
        private static bool firstLoad = true;

        public PasswordsPage()
        {
            this.InitializeComponent();
            App.m_window.passwordItemsManager = passwordItemsManager;
            App.m_window.Closed += M_window_Closed;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            App.m_window.ShowBackArrow = false;

            if (e.NavigationMode == NavigationMode.Back)
            {
                SaveData();
            }
            else if(e.NavigationMode == NavigationMode.New && e.Parameter is SecureString pw)
            {
                masterPassword = pw;
                InfobarExtension.ClearInfobarsAfterLogin(MainWindow.InfoMessagesPanel);
                AppVersionHelper.CheckNewVersion();

                //load the gridsplittervalue back
                gridSplitterLoadSize.Width = new GridLength(AppSettings.GetSettingsAsInt(AppSettingsValues.gridSplitterWidth, 240), GridUnitType.Pixel);
            }

            if (passwordItemsManager.PasswordItems == null)
            {
                var dbData = DatabaseHelper.LoadDatabase(masterPassword);
                if (dbData == null)
                    dbData = new ObservableCollection<PasswordManagerItem>();

                for(int i = 0; i < dbData.Count; i++)
                {
                    for(int j = 0; j < dbData[i].Clicks.Count; j++)
                    {
                        string[] splitted = dbData[i].Clicks[j].Split('.');
                        DateTime date = new DateTime(Convert.ToInt32(splitted[2]), Convert.ToInt32(splitted[1]), Convert.ToInt32(splitted[0]));
                        if(DateTime.Now - date > TimeSpan.FromDays(365))
                        {
                            dbData[i].Clicks.RemoveAt(j);
                            j--;
                        }
                    }
                }

                passwordItemsManager.Load(dbData);
                passwordItemListView.ItemsSource = passwordItemsManager.PasswordItems;

                //enable backups:
                MainWindow.databaseBackupHelper = new DatabaseBackupHelper(passwordItemsManager, masterPassword, BackupCycle.Daily);
                await MainWindow.databaseBackupHelper.CheckAndDoBackup();

                SaveData();
            }

            if (firstLoad)
            {
                firstLoad = false;
                ShowPasswordItem(new PasswordManagerItem()
                {
                    DisplayName = "",
                    Username = "",
                    Password = "",
                    Email = "",
                    Notes = ""
                }, true);
            }

            base.OnNavigatedTo(e);
        }
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            StoreGridSplitterValue();
        }

        public void Reload()
        {
            passwordItemListView.ItemsSource = null;
            passwordItemListView.ItemsSource = passwordItemsManager.PasswordItems;
        }
        public void SaveData()
        {
            DatabaseHelper.SaveDatabase(passwordItemsManager, masterPassword);
        }
        private void ShowPasswordItem(PasswordManagerItem item, bool dummy = false)
        {
            if (item == null)
                return;

            passwordShowArea.Visibility = Visibility.Visible;

            pwTB.SetPasswordItems(passwordItemsManager);

            notesTB.Text = item.Notes;
            pwTB.Password = item.Password;
            emailTB.Text = item.Email;
            usernameTB.Text = item.Username;
            itemnameTB.Text = item.DisplayName;
            websiteTB.Text = item.Website;

            if (dummy)
            {
                DispatcherTimer timer = new DispatcherTimer();
                timer.Interval = TimeSpan.FromSeconds(0.5);
                timer.Tick += (object sender, object e) =>
                {
                    passwordShowArea.Visibility = Visibility.Collapsed;
                    (sender as DispatcherTimer).Stop();
                };
                timer.Start();
                return;
            }

            if (!string.IsNullOrEmpty(item.Secret))
            {
                totpTB.Visibility = totpLB.Visibility = Visibility.Visible;
                if (totpTokenUpdater != null)
                    totpTokenUpdater.StopTimer();
                totpTokenUpdater = new TOTPTokenUpdater(item, totpTB);
                totpTokenUpdater.StartTimer();
                totpTokenUpdater.SimulateTickEvent();
                return;
            }

            totpTB.Visibility = totpLB.Visibility = Visibility.Collapsed;
            if(totpTokenUpdater != null)
                totpTokenUpdater.StopTimer();
        }
        private async Task DeletePasswordItem(PasswordManagerItem deleteItem)
        {
            if (deleteItem == null)
                return;

            if (await new DeleteConfirmationDialog().ShowAsync(deleteItem))
            {
                int index = passwordItemListView.SelectedIndex;
                passwordItemsManager.DeleteItem(deleteItem);

                if (passwordItemListView.Items.Count >= 1)
                    passwordItemListView.SelectedIndex = index - 1 > 0 ? index - 1 : index + 1 < passwordItemListView.Items.Count ? index + 1 : 0;
                else
                    passwordShowArea.Visibility = Visibility.Collapsed;

                //update searchbox:
                if (searchbox.Text.Length > 0)
                {
                    ObservableCollection<PasswordManagerItem> items = passwordItemsManager.FindItemsByName(searchbox.Text);
                    passwordItemListView.ItemsSource = items;
                }

                SaveData();
            }
        }
        private async Task DeletePasswordItems(PasswordManagerItem[] deleteItems)
        {
            if (deleteItems == null || deleteItems.Length == 0) 
                return;

            if (await new DeleteConfirmationDialog().ShowAsync(deleteItems))
            {
                foreach (var item in deleteItems)
                {
                    passwordItemsManager.DeleteItem(item);
                }

                if (passwordItemListView.Items.Count >= 1)
                    passwordItemListView.SelectedIndex = 0;
                else
                    passwordShowArea.Visibility = Visibility.Collapsed;

                //update searchbox:
                if (searchbox.Text.Length > 0)
                {
                    ObservableCollection<PasswordManagerItem> items = passwordItemsManager.FindItemsByName(searchbox.Text);
                    passwordItemListView.ItemsSource = items;
                }

                SaveData();
            }
        }
        private async Task EditPasswordItem(PasswordManagerItem item)
        {
            if (item == null)
                return;

            var editItem = await new EditItemDialog().ShowAsync(passwordItemsManager.PasswordAlreadyExists, item);
            if (editItem == null)
                return;

            ShowPasswordItem(SelectedItem);
            SaveData();
        }
        private async Task AddPasswordItem()
        {
            var newItem = await new AddItemDialog().ShowAsync(passwordItemsManager.PasswordAlreadyExists);
            if (newItem == null)
                return;

            passwordItemsManager.AddItem(newItem);
            Searchbox_TextChanged(searchbox, null);
            SaveData();
        }
        private async Task Add2FAPasswordItem(PasswordManagerItem item)
        {
            if (item == null)
                return;

            if (!await new Add2FADialog().ShowAsync(item))
                return;

            try
            {
                TOTP.GenerateTOTPToken(DateTime.Now, item.Secret, Convert.ToInt32(item.Digits), Convert.ToInt32(item.Interval), TOTP.StringToHashMode(item.Algorithm));
            }
            catch
            {
                item.Secret = "";
                InfoMessages.Invalid2FA();
            }

            ShowPasswordItem(item);
            SaveData();
        }
        private async Task GeneratePassword()
        {
            //returns true when the regenerate button was pressed
            if (await new GenPasswordDialog().ShowAsync(passwordItemsManager))
                await GeneratePassword();
        }
        private void ShowSettingsPage()
        {
            App.m_frame.Navigate(typeof(SettingsPage), new SettingsNavigationParameters
            {
                PasswordPage = this,
                PwItemsManager = passwordItemsManager,
                SavePwItems = SaveData
            });
        }
        private void SetVis(FontIcon icon)
        {
            sortname.Visibility = ConvertHelper.BoolToVisibility(icon == sortname);
            sortusername.Visibility = ConvertHelper.BoolToVisibility(icon == sortusername);
            sortnotes.Visibility = ConvertHelper.BoolToVisibility(icon == sortnotes);
            sortwebsite.Visibility = ConvertHelper.BoolToVisibility(icon == sortwebsite);
        }
        private void StoreGridSplitterValue()
        {
            AppSettings.SaveSettings(AppSettingsValues.gridSplitterWidth, gridSplitterLoadSize.Width);
        }

        private void M_window_Closed(object sender, WindowEventArgs args)
        {
            StoreGridSplitterValue();
        }
        private async void DeletePasswordItem_Click(object sender, RoutedEventArgs e)
        {
            if (passwordItemListView.SelectedItems.Count == 1)
            {
                await DeletePasswordItem(SelectedItem);
                return;
            }

            await DeletePasswordItems(passwordItemListView.SelectedItems.Select(x => x as PasswordManagerItem).ToArray());
        }
        private async void AddPasswordItem_Click(object sender, RoutedEventArgs e) => await AddPasswordItem();
        private async void EditPasswordItem_Click(object sender, RoutedEventArgs e) => await EditPasswordItem(SelectedItem);
        private async void Add2FAPasswordItem_Click(object sender, RoutedEventArgs e) => await Add2FAPasswordItem(SelectedItem);
        private async void GenPassword_Click(object sender, RoutedEventArgs e) => await GeneratePassword();
        private void Settings_Click(object sender, RoutedEventArgs e) => ShowSettingsPage();
        private void AboutPage_Click(object sender, RoutedEventArgs e)
        {
            App.m_frame.Navigate(typeof(AboutPage));
        }

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
                SelectedItem.Clicks.Add(DateTime.Now.ToString("d").Replace("/", "."));
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
                passwordItemListView.ItemsSource = passwordItemsManager.PasswordItems;
                searchbox.InfoLabel = passwordItemListView.Items.Count.ToString();
                return;
            }
            var search_res = passwordItemsManager.FindItemsByName(searchbox.Text);
            passwordItemListView.ItemsSource = search_res;
            searchbox.InfoLabel = passwordItemListView.Items.Count.ToString();
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
            searchbox.InfoLabel = passwordItemListView.Items.Count.ToString();
        }
        private void Page_KeyDown(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (KeyHelper.IsKeyPressed(Windows.System.VirtualKey.Control) && !KeyHelper.IsKeyPressed(Windows.System.VirtualKey.Menu))
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
                        if (passwordItemListView.SelectedIndex > 0 && passwordItemListView.SelectedIndex < passwordItemListView.Items.Count)
                            passwordItemListView.SelectedIndex--;
                        break;
                    case Windows.System.VirtualKey.L:
                        LogoutHelper.Logout();
                        break;
                    default: 
                        return;
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

        private void SortName_Click(object sender, RoutedEventArgs e) => SortClickAction(SortingHelper.ByDisplayName, sortname);
        private void SortUsername_Click(object sender, RoutedEventArgs e) => SortClickAction(SortingHelper.ByUsername, sortusername);
        private void SortNotes_Click(object sender, RoutedEventArgs e) => SortClickAction(SortingHelper.ByNotes, sortnotes);
        private void SortWebsite_Click(object sender, RoutedEventArgs e) => SortClickAction(SortingHelper.ByWebsite, sortwebsite);
        private void SortPopularAll_Click(object sender, RoutedEventArgs e) => SortClickAction(SortingHelper.ByPopularAllTime, sortpopularall); // "popular all time" is equal to "popular last year" to save storage space
        private void SortPopular30_Click(object sender, RoutedEventArgs e) => SortClickAction(SortingHelper.ByPopularLast30Days, sortpopular30);
        private void SortPasswordStrength(object sender, RoutedEventArgs e) => SortClickAction(SortingHelper.ByPasswordStrength, sortpasswordstrength);
        private void SortClickAction(Comparison<PasswordManagerItem> comparison, FontIcon icon)
        {
            passwordItemsManager.PasswordItems.Sort(comparison);
            Reload();
            SaveData();
            SetVis(icon);
            Searchbox_TextChanged(this, null);
        }
        private void SwitchOrder_Click(object sender, RoutedEventArgs e)
        {
            passwordItemsManager.SetNew(passwordItemsManager.PasswordItems.ReverseSelf());
            Reload();
            SaveData();
            Searchbox_TextChanged(this, null);
        }

        private void RightclickedItem_CopyUsername_Click(object sender, RoutedEventArgs e) => ClipboardHelper.Copy(((sender as MenuFlyoutItem)?.Tag as PasswordManagerItem)?.Username);
        private void RightclickedItem_CopyEmail_Click(object sender, RoutedEventArgs e) => ClipboardHelper.Copy(((sender as MenuFlyoutItem)?.Tag as PasswordManagerItem)?.Email);
        private void RightclickedItem_CopyPassword_Click(object sender, RoutedEventArgs e) => ClipboardHelper.Copy(((sender as MenuFlyoutItem)?.Tag as PasswordManagerItem)?.Password);
        private async void RightclickedItem_Delete_Click(object sender, RoutedEventArgs e) => await DeletePasswordItem((sender as MenuFlyoutItem)?.Tag as PasswordManagerItem);
        private async void RightclickedItem_Edit_Click(object sender, RoutedEventArgs e) => await EditPasswordItem((sender as MenuFlyoutItem)?.Tag as PasswordManagerItem);
        private void RightclickedItem_CopyTOTPToken_Click(object sender, RoutedEventArgs e) => ClipboardHelper.Copy(TOTPTokenUpdater.generateCurrent((sender as MenuFlyoutItem)?.Tag as PasswordManagerItem).Replace(" ", ""));
    }
}
