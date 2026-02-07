using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.VisualTree;
using EasePass.Controls;
using EasePass.Core.Database;
using EasePass.Dialogs;
using EasePass.Extensions;
using EasePass.Helper;
using EasePass.Helper.App;
using EasePass.Helper.Database;
using EasePass.Helper.Logout;
using EasePass.Helper.Security;
using EasePass.Helper.Security.Generator;
using EasePass.Manager;
using EasePass.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace EasePass.Views
{
    public partial class PasswordsPage : UserControl, INotifyPropertyChanged
    {
        public delegate int PasswordExists(string password);
        private PasswordManagerItem _SelectedItem = null;
        public PasswordManagerItem SelectedItem
        {
            get => _SelectedItem;
            set
            {
                _SelectedItem = value;
                RaisePropertyChanged(nameof(SelectedItem));
            }
        }

        private TOTPTokenUpdater totpTokenUpdater;
        private SearchPasswordsManager searchPwManager = new SearchPasswordsManager();
        public DatabaseItem LoadedDB => Database.LoadedInstance;

        public PasswordsPage()
        {
            InitializeComponent();
            this.DataContext = this;

            if (MainWindow.current != null)
            {
                MainWindow.current.Closed += M_window_Closed;
            }
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public new event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private void LoadedDB_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsReadonlyDatabase")
            {
                RaisePropertyChanged(nameof(LoadedDB));
            }
        }

        private void LoadControls()
        {
            passwordItemListView = this.FindControl<ListBox>("passwordItemListView");
            if (passwordItemListView == null)
                throw new Exception("PasswordItemListView is null");

            searchbox = this.FindControl<SearchPasswordsBox>("searchbox");
            if (searchbox == null)
                throw new Exception("Searchbox must not be null");

            pwTB = this.FindControl<TextBox>("pwTB");
            if(pwTB == null)
                throw new Exception("Searchbox must not be null");
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadControls();

            UpdateOOBEGrid();
            searchPwManager.tagSearchManager.UniqueTagList?.Clear();

           
            searchbox.InfoLabel = passwordItemListView.Items.Count.ToString();
            searchbox.Focus(NavigationMethod.Unspecified);

            // Check references for controls
            var infoPanel = MainWindow.InfoMessagesPanel;
            if (infoPanel != null)
                InfobarExtension.ClearInfobarsAfterLogin(infoPanel);

            AppVersionHelper.CheckNewVersion();

            var grid = this.FindControl<Grid>("MainGrid"); // Assuming root grid has name? No name in xaml.

            //todo
            //var colDef = this.FindControl<ColumnDefinition>("gridSplitterLoadSize");
            //if (colDef != null)
            //{
            //     colDef.Width = new GridLength(AppSettings.GridSplitterWidth, GridUnitType.Pixel);
            //}

            if (Database.LoadedInstance != null)
            {

                passwordItemListView.ItemsSource = Database.LoadedInstance.Items;

                var loadTempDBButton = this.FindControl<Button>("loadTempDBButton");
                if (loadTempDBButton != null)
                    TemporaryDatabaseHelper.ShowTempDBButton(loadTempDBButton);

                LoadedDB.PropertyChanged += LoadedDB_PropertyChanged;
            }


            UpdateSearchbox();
        }


        public void Reload()
        {
            passwordItemListView.ItemsSource = null;
            passwordItemListView.ItemsSource = Database.LoadedInstance.Items;
        }

        private async Task DeletePasswordItem(PasswordManagerItem deleteItem)
        {
            if (deleteItem == null)
                return;

            if (await new DeleteConfirmationDialog().ShowAsync(deleteItem))
            {
                int index = passwordItemListView.SelectedIndex;
                Database.LoadedInstance.DeleteItem(deleteItem);

                if (passwordItemListView.Items.Count >= 1)
                    passwordItemListView.SelectedIndex = index - 1 > 0 ? index - 1 : index + 1 < passwordItemListView.Items.Count ? index + 1 : 0;
                else
                    passwordEntryTitle.IsVisible = passwordShowArea.IsVisible = false;

                //update searchbox:
                if (searchbox.Text.Length > 0)
                {
                    ObservableCollection<PasswordManagerItem> items = Database.LoadedInstance.FindItemsByName(searchbox.Text);
                    passwordItemListView.ItemsSource = items;
                }

                await Database.LoadedInstance.SaveAsync();
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
                    Database.LoadedInstance.DeleteItem(item);
                }

                if (passwordItemListView.Items.Count >= 1)
                    passwordItemListView.SelectedIndex = 0;
                else
                {
                    passwordEntryTitle.IsVisible = passwordShowArea.IsVisible = false;
                }

                //update searchbox:
                if (searchbox.Text.Length > 0)
                {
                    ObservableCollection<PasswordManagerItem> items = Database.LoadedInstance.FindItemsByName(searchbox.Text);
                    passwordItemListView.ItemsSource = items;
                }

                await Database.LoadedInstance.SaveAsync();
            }
        }
        private async Task EditPasswordItem(PasswordManagerItem item)
        {
            if (item == null)
                return;

            PasswordManagerItem editItem = await new EditItemDialog().ShowAsync(Database.LoadedInstance.GetPasswordOccurence, item);
            if (editItem == null)
                return;

            await Database.LoadedInstance.SaveAsync();
        }
        private async Task AddPasswordItem()
        {
            PasswordManagerItem newItem = await new AddItemDialog().ShowAsync(Database.LoadedInstance.GetPasswordOccurence);
            if (newItem == null)
                return;

            Database.LoadedInstance.AddItem(newItem);
            UpdateSearchbox();
            await Database.LoadedInstance.SaveAsync();
        }
        private void Update2FATimer()
        {
            if (SelectedItem == null)
                return;

            if (totpTokenUpdater != null)
                totpTokenUpdater.StopTimer();

            if (!string.IsNullOrEmpty(SelectedItem.Secret))
            {
                totpTokenUpdater = new TOTPTokenUpdater(SelectedItem, totpTB);
                totpTokenUpdater.StartTimer();
                totpTokenUpdater.SimulateTickEvent();
            }
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

            Update2FATimer();
            await Database.LoadedInstance.SaveAsync();
        }
        private async Task GeneratePassword()
        {
            //returns true when the Dialog was Closed
            await new GenPasswordDialog().ShowAsync();
        }

        private void StoreGridSplitterValue()
        {
            //todo var colDef = this.FindControl<ColumnDefinition>("gridSplitterLoadSize");
            //if (colDef != null)
            //AppSettings.GridSplitterWidth = (int)colDef.Width.Value;
        }

        private void UpdateOOBEGrid()
        {
            var oobe_Grid = this.FindControl<Grid>("oobe_Grid");
            if (oobe_Grid != null && Database.LoadedInstance != null)
                oobe_Grid.IsVisible = Database.LoadedInstance.Items.Count == 0;
        }

        private void M_window_Closed(object sender, EventArgs args)
        {
            StoreGridSplitterValue();
        }
        private async Task CopyTOTPToken(PasswordManagerItem pwItem)
        {
            if (pwItem.Secret == null)
                return;

            var generated = await TOTPTokenUpdater.generateCurrent(pwItem);
            await ClipboardHelper.CopyAsync(generated.Replace(" ", ""));
        }

        private void UpdateSearchbox()
        {
            Searchbox_TextChanged(null, true, searchbox.Text);
        }

        private async void DeletePasswordItem_Click(object sender, RoutedEventArgs e)
        {
            if (passwordItemListView.SelectedItems.Count == 1)
            {
                await DeletePasswordItem(SelectedItem);
                return;
            }

            await DeletePasswordItems((passwordItemListView.SelectedItems as List<PasswordManagerItem>).ToArray());
        }
        private async void AddPasswordItem_Click(object sender, RoutedEventArgs e) => await AddPasswordItem();
        private async void EditPasswordItem_Click(object sender, RoutedEventArgs e) => await EditPasswordItem(SelectedItem);
        private async void Add2FAPasswordItem_Click(object sender, RoutedEventArgs e) => await Add2FAPasswordItem(SelectedItem);
        private async void GenPassword_Click(object sender, RoutedEventArgs e) => await GeneratePassword();
        private void Settings_Click(object sender, RoutedEventArgs e) => NavigationHelper.ToSettings();
        private void AboutPage_Click(object sender, RoutedEventArgs e)
        {
            NavigationHelper.ToAboutPage();
        }

        private void PasswordItemListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateOOBEGrid();
            if (passwordItemListView.Items.Count == 0)
            {
                passwordEntryTitle.IsVisible = passwordShowArea.IsVisible = false;
            }

            //directly select the item for the user
            if (passwordItemListView.SelectedItems.Count == 1)
                passwordItemListView.SelectedItem = passwordItemListView.SelectedItems[0];

            if (passwordItemListView.SelectedItem == null)
            {
                SelectedItem = null;
                return;
            }

            if (passwordItemListView.SelectedItem is PasswordManagerItem pwItem)
            {
                SelectedItem = pwItem;
                SelectedItem.Clicks.Add(DateTime.Now.ToString("d").Replace("/", "."));
                pwTB.RevealPassword = false;
                Update2FATimer();
            }
        }
        //todo: private async void PasswordItemListView_DragItemsCompleted(ListViewBase sender, DragItemsCompletedEventArgs args)
        //{
        //    if (args.Items.Count > 0)
        //        await Database.LoadedInstance.SaveAsync();
        //}

        private void Page_KeyDown(object sender, KeyEventArgs e)
        {

            if (e.KeyModifiers == KeyModifiers.Control && e.KeyModifiers != KeyModifiers.Alt)
            {
                switch (e.Key)
                {

                    case Key.F:
                        searchbox.Focus(NavigationMethod.Unspecified);
                        break;
                    case Key.N:
                        AddPasswordItem_Click(null, null);
                        break;
                    case Key.E:
                        EditPasswordItem_Click(null, null);
                        break;
                    case Key.Down:
                        if (passwordItemListView.SelectedIndex >= 0 && passwordItemListView.SelectedIndex < passwordItemListView.Items.Count - 1)
                            passwordItemListView.SelectedIndex++;
                        break;
                    case Key.Up:
                        if (passwordItemListView.SelectedIndex > 0 && passwordItemListView.SelectedIndex < passwordItemListView.Items.Count)
                            passwordItemListView.SelectedIndex--;
                        break;
                    case Key.L:
                        LogoutHelper.Logout();
                        break;
                    default:
                        return;
                }
            }

            switch (e.Key)
            {
                case Key.F1:
                    Settings_Click(null, null);
                    break;
                case Key.F2:
                    EditPasswordItem_Click(null, null);
                    break;
                case Key.Escape:
                    passwordItemListView.SelectedItem = SelectedItem = null;
                    break;
            }
        }

        private void SortName_Click(object sender, RoutedEventArgs e) => SortClickAction(SortingHelper.ByDisplayName, sender);
        private void SortUsername_Click(object sender, RoutedEventArgs e) => SortClickAction(SortingHelper.ByUsername, sender);
        private void SortNotes_Click(object sender, RoutedEventArgs e) => SortClickAction(SortingHelper.ByNotes, sender);
        private void SortWebsite_Click(object sender, RoutedEventArgs e) => SortClickAction(SortingHelper.ByWebsite, sender);
        private void SortPopularAll_Click(object sender, RoutedEventArgs e) => SortClickAction(SortingHelper.ByPopularAllTime, sender); // "popular all time" is equal to "popular last year" to save storage space
        private void SortPopular30_Click(object sender, RoutedEventArgs e) => SortClickAction(SortingHelper.ByPopularLast30Days, sender);
        private void SortPasswordStrength(object sender, RoutedEventArgs e) => SortClickAction(SortingHelper.ByPasswordStrength, sender);
        private void SortTags_Click(object sender, RoutedEventArgs e) => SortClickAction(SortingHelper.ByFirstTag, sender);

        private async void SortClickAction(Comparison<PasswordManagerItem> comparison, object sender)
        {
            //always check the current sorting method
            var ignoreItem = sender as MenuItem;
            var sortPasswordListFlyout = SortPasswordListButton.Flyout as MenuFlyout;
            for (int i = 0; i < sortPasswordListFlyout.Items.Count; i++)
            {
                //if (sortPasswordListFlyout.Items[i] is MenuFlyout iItem)
                //    iItem. = iItem == ignoreItem;
            }

            Database.LoadedInstance.Items.Sort(comparison);
            Reload();
            await Database.LoadedInstance.SaveAsync();
        }
        private async void SwitchOrder_Click(object sender, RoutedEventArgs e)
        {
            Database.LoadedInstance.SetNewPasswords(Database.LoadedInstance.Items.ReverseSelf());
            Reload();
            await Database.LoadedInstance.SaveAsync();
            UpdateSearchbox();
        }

        private void RightclickedItem_CopyUsername_Click(object sender, RoutedEventArgs e) => ClipboardHelper.CopyAsync(((sender as MenuItem)?.Tag as PasswordManagerItem)?.Username);
        private void RightclickedItem_CopyEmail_Click(object sender, RoutedEventArgs e) => ClipboardHelper.CopyAsync(((sender as MenuItem)?.Tag as PasswordManagerItem)?.Email);
        private void RightclickedItem_CopyPassword_Click(object sender, RoutedEventArgs e) => ClipboardHelper.CopyAsync(((sender as MenuItem)?.Tag as PasswordManagerItem)?.Password, true);
        private async void RightclickedItem_Delete_Click(object sender, RoutedEventArgs e)
        {
            if (LoadedDB == null || LoadedDB.IsReadonlyDatabase)
                return;

            if (passwordItemListView.SelectedItems.Count > 1)
            {
                await DeletePasswordItems((passwordItemListView.SelectedItems as List<PasswordManagerItem>).ToArray());
                return;
            }

            //single item was right clicked, does not have to be the selected item:
            await DeletePasswordItem((sender as MenuItem)?.Tag as PasswordManagerItem);
        }
        private async void RightclickedItem_Edit_Click(object sender, RoutedEventArgs e)
        {
            if (LoadedDB != null && !LoadedDB.IsReadonlyDatabase)
                await EditPasswordItem((sender as MenuItem)?.Tag as PasswordManagerItem);
        }
        private async void RightclickedItem_CopyTOTPToken_Click(object sender, RoutedEventArgs e)
        {
            await CopyTOTPToken((sender as MenuItem)?.Tag as PasswordManagerItem);
        }
        private async void RightclickedItem_ExportSelected_Click(object sender, RoutedEventArgs e)
        {
            if (passwordItemListView.SelectedItems.Count > 1)
            {
                //multiple items are selected => export them
                var items = new ObservableCollection<PasswordManagerItem>(
                    passwordItemListView.SelectedItems as List<PasswordManagerItem>);

                await ExportPasswordsHelper.Export(Database.LoadedInstance, items);
                return;
            }

            //single item was right clicked, does not have to be the selected item:
            await ExportPasswordsHelper.Export(Database.LoadedInstance, new ObservableCollection<PasswordManagerItem>() { (sender as MenuItem)?.Tag as PasswordManagerItem });
        }
        private async void RightclickedItem_ExportDiffPassword_Click(object sender, RoutedEventArgs e)
        {
            var dialog = await new EnterPasswordDialog().ShowAsync();
            if (dialog.Password == null)
                return;

            if (passwordItemListView.SelectedItems.Count > 1)
            {
                //multiple items are selected => export them
                var items = new ObservableCollection<PasswordManagerItem>(
                    passwordItemListView.SelectedItems as List<PasswordManagerItem>);
                await ExportPasswordsHelper.Export(Database.LoadedInstance, items, dialog.Password, false);
                return;
            }

            //single item was right clicked, does not have to be the selected item:
            await ExportPasswordsHelper.Export(Database.LoadedInstance, new ObservableCollection<PasswordManagerItem>() { (sender as MenuItem)?.Tag as PasswordManagerItem }, dialog.Password, false);
        }

        private async void Grid_Drop(object sender, DragEventArgs e)
        {
            await DatabaseDragDropHelper.Drop(e);
        }
        private void Grid_DragOver(object sender, DragEventArgs e)
        {
            DatabaseDragDropHelper.DragOver(e);
        }
        private void OOBE_HyperlinkManageDB(object sender, RoutedEventArgs e)
        {
            NavigationHelper.ToManageDB();
        }
        private void LoadTemporaryDatabase_Click(object sender, RoutedEventArgs e)
        {
            TemporaryDatabaseHelper.LoadImportedDatabase();
            loadTempDBButton.IsVisible = false;
        }

        private async void LogOut_Click(object sender, RoutedEventArgs e)
        {
            //first save the database, then logout
            await MainWindow.current.DoMasterSaveWithProgress();
            LogoutHelper.Logout();
        }

        private void Searchbox_TextChanged(object sender, bool isUserTextChange, string text)
        {
            if (passwordItemListView == null)
                return;

            if (string.IsNullOrEmpty(text))
            {
                passwordItemListView.ItemsSource = Database.LoadedInstance.Items;
                searchbox.InfoLabel = Database.LoadedInstance.Items.Count.ToString();
                return;
            }

            var searchRes = searchPwManager.SearchPasswords(searchbox, Database.LoadedInstance, isUserTextChange, text);
            if (searchRes == null)
                return;

            passwordItemListView.ItemsSource = searchRes;
            if (searchRes.Count == 1)
            {
                passwordItemListView.SelectedItem = searchRes[0];
            }
        }

        private void Searchbox_PreviewKeyDown(bool isTagSearch, KeyEventArgs e)
        {
            //if (searchbox.InternalSuggestBox.IsSuggestionListOpen && isTagSearch)
            //return;

            if (e.Key == Key.Down)
            {
                e.Handled = true;
                passwordItemListView.Focus(NavigationMethod.Unspecified);
                if (passwordItemListView.SelectedIndex == -1)
                {
                    passwordItemListView.SelectedIndex = 0;
                }
            }
        }

        private void SearchBox_GotFocus(object sender, RoutedEventArgs e)
        {
            //index tags for search box
            if (Database.LoadedInstance == null)
                return;

            searchPwManager.tagSearchManager.UpdateTagList(Database.LoadedInstance);
        }
    }
}
