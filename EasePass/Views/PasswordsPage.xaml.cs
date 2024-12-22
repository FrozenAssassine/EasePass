/*
MIT License

Copyright (c) 2023 Julius Kirsch

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.
*/

using EasePass.Core.Database;
using EasePass.Dialogs;
using EasePass.Extensions;
using EasePass.Helper;
using EasePass.Models;
using EasePass.Settings;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;

namespace EasePass.Views
{
    public sealed partial class PasswordsPage : Page, INotifyPropertyChanged
    {
        public delegate int PasswordExists(string password);
        public const int TOTP_SPACING = 3;
        private PasswordManagerItem _SelectedItem = null;
        private PasswordManagerItem SelectedItem { get => _SelectedItem; set { _SelectedItem = value; RaisePropertyChanged("SelectedItem"); } }
        private TOTPTokenUpdater totpTokenUpdater;

        public PasswordsPage()
        {
            this.InitializeComponent();
            App.m_window.Closed += M_window_Closed;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            App.m_window.ShowBackArrow = false;
            UpdateOOBEGrid();

            if (e.NavigationMode == NavigationMode.Back)
            {
                Database.LoadedInstance.Save();
            }
            else if (e.NavigationMode == NavigationMode.New)
            {
                InfobarExtension.ClearInfobarsAfterLogin(MainWindow.InfoMessagesPanel);
                AppVersionHelper.CheckNewVersion();

                gridSplitterLoadSize.Width = new GridLength(AppSettings.GridSplitterWidth, GridUnitType.Pixel);
            }

            if (Database.LoadedInstance != null)
            {
                passwordItemListView.ItemsSource = Database.LoadedInstance.Items;
                Database.LoadedInstance.Save();

                TemporaryDatabaseHelper.ShowTempDBButton(loadTempDBButton);
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
                    passwordShowArea.Visibility = Visibility.Collapsed;

                //update searchbox:
                if (searchbox.Text.Length > 0)
                {
                    ObservableCollection<PasswordManagerItem> items = Database.LoadedInstance.FindItemsByName(searchbox.Text);
                    passwordItemListView.ItemsSource = items;
                }

                Database.LoadedInstance.Save();
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
                    passwordShowArea.Visibility = Visibility.Collapsed;

                //update searchbox:
                if (searchbox.Text.Length > 0)
                {
                    ObservableCollection<PasswordManagerItem> items = Database.LoadedInstance.FindItemsByName(searchbox.Text);
                    passwordItemListView.ItemsSource = items;
                }

                Database.LoadedInstance.Save();
            }
        }
        private async Task EditPasswordItem(PasswordManagerItem item)
        {
            if (item == null)
                return;

            var editItem = await new EditItemDialog().ShowAsync(Database.LoadedInstance.PasswordAlreadyExists, item);
            if (editItem == null)
                return;

            Database.LoadedInstance.Save();
        }
        private async Task AddPasswordItem()
        {
            var newItem = await new AddItemDialog().ShowAsync(Database.LoadedInstance.PasswordAlreadyExists);
            if (newItem == null)
                return;

            Database.LoadedInstance.AddItem(newItem);
            Searchbox_TextChanged(searchbox, null);
            Database.LoadedInstance.Save();
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
            Database.LoadedInstance.Save();
        }
        private async Task GeneratePassword()
        {
            //returns true when the Dialog was Closed
            await new GenPasswordDialog().ShowAsync();
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
            AppSettings.GridSplitterWidth = (int)gridSplitterLoadSize.Width.Value;
        }
        private void UpdateOOBEGrid()
        {
            oobe_Grid.Visibility = ConvertHelper.BoolToVisibility(Database.LoadedInstance.Items.Count == 0);
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
        private void Settings_Click(object sender, RoutedEventArgs e) => NavigationHelper.ToSettings(this);
        private void AboutPage_Click(object sender, RoutedEventArgs e)
        {
            NavigationHelper.ToAboutPage();
        }

        private void PasswordItemListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateOOBEGrid();
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
                Update2FATimer();
            }
        }
        private void PasswordItemListView_DragItemsCompleted(ListViewBase sender, DragItemsCompletedEventArgs args)
        {
            if (args.Items.Count > 0)
                Database.LoadedInstance.Save();
        }

        private void Searchbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (searchbox.Text.Length == 0)
            {
                passwordItemListView.ItemsSource = Database.LoadedInstance.Items;
                searchbox.InfoLabel = passwordItemListView.Items.Count.ToString();
                return;
            }
            var search_res = Database.LoadedInstance.FindItemsByName(searchbox.Text);
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
            Database.LoadedInstance.Items.Sort(comparison);
            Reload();
            Database.LoadedInstance.Save();
            SetVis(icon);
            Searchbox_TextChanged(this, null);
        }
        private void SwitchOrder_Click(object sender, RoutedEventArgs e)
        {
            Database.LoadedInstance.SetNewPasswords(Database.LoadedInstance.Items.ReverseSelf());
            Reload();
            Database.LoadedInstance.Save();
            Searchbox_TextChanged(this, null);
        }

        private void RightclickedItem_CopyUsername_Click(object sender, RoutedEventArgs e) => ClipboardHelper.Copy(((sender as MenuFlyoutItem)?.Tag as PasswordManagerItem)?.Username);
        private void RightclickedItem_CopyEmail_Click(object sender, RoutedEventArgs e) => ClipboardHelper.Copy(((sender as MenuFlyoutItem)?.Tag as PasswordManagerItem)?.Email);
        private void RightclickedItem_CopyPassword_Click(object sender, RoutedEventArgs e) => ClipboardHelper.Copy(((sender as MenuFlyoutItem)?.Tag as PasswordManagerItem)?.Password, true);
        private async void RightclickedItem_Delete_Click(object sender, RoutedEventArgs e)
        {
            if (passwordItemListView.SelectedItems.Count > 1)
            {
                await DeletePasswordItems(passwordItemListView.SelectedItems.Select(x => x as PasswordManagerItem).ToArray());
                return;
            }

            //single item was right clicked, does not have to be the selected item:
            await DeletePasswordItem((sender as MenuFlyoutItem)?.Tag as PasswordManagerItem);
        }
        private async void RightclickedItem_Edit_Click(object sender, RoutedEventArgs e) => await EditPasswordItem((sender as MenuFlyoutItem)?.Tag as PasswordManagerItem);
        private async void RightclickedItem_CopyTOTPToken_Click(object sender, RoutedEventArgs e) => ClipboardHelper.Copy((await TOTPTokenUpdater.generateCurrent((sender as MenuFlyoutItem)?.Tag as PasswordManagerItem)).Replace(" ", ""));
        private async void RightclickedItem_ExportSelected_Click(object sender, RoutedEventArgs e)
        {
            //single item was right clicked, does not have to be the selected item:

            if (passwordItemListView.SelectedItems.Count > 1)
            {
                //multiple items are selected => export them
                var items = new ObservableCollection<PasswordManagerItem>(passwordItemListView.SelectedItems.Select(x => (PasswordManagerItem)x).ToList());
                await ExportPasswordsHelper.Export(Database.LoadedInstance, items);
                return;
            }

            await ExportPasswordsHelper.Export(Database.LoadedInstance, new ObservableCollection<PasswordManagerItem>() { (sender as MenuFlyoutItem)?.Tag as PasswordManagerItem });
        }
        private async void RightclickedItem_ExportDiffPassword_Click(object sender, RoutedEventArgs e)
        {
            var dialog = await new EnterPasswordDialog().ShowAsync();
            if (dialog.Password == null)
                return;

            //single item was right clicked, does not have to be the selected item:
            if (passwordItemListView.SelectedItems.Count > 1)
            {
                //multiple items are selected => export them
                var items = new ObservableCollection<PasswordManagerItem>(passwordItemListView.SelectedItems.Select(x => (PasswordManagerItem)x).ToList());
                await ExportPasswordsHelper.Export(Database.LoadedInstance, items, dialog.Password, false);
                return;
            }

            await ExportPasswordsHelper.Export(Database.LoadedInstance, new ObservableCollection<PasswordManagerItem>() { (sender as MenuFlyoutItem)?.Tag as PasswordManagerItem }, dialog.Password, false);
        }

        private async void Grid_Drop(object sender, DragEventArgs e)
        {
            await DatabaseDragDropHelper.Drop(e);
        }
        private void Grid_DragOver(object sender, DragEventArgs e)
        {
            DatabaseDragDropHelper.DragOver(e);
        }

        private void EscapeKeyAccel_Invoked(Microsoft.UI.Xaml.Input.KeyboardAccelerator sender, Microsoft.UI.Xaml.Input.KeyboardAcceleratorInvokedEventArgs args)
        {
            passwordItemListView.SelectedItem = SelectedItem = null;
        }

        private void OOBE_HyperlinkSettings(Microsoft.UI.Xaml.Documents.Hyperlink sender, Microsoft.UI.Xaml.Documents.HyperlinkClickEventArgs args)
        {
            NavigationHelper.ToSettings(this);
        }

        private void OOBE_HyperlinkManageDB(Microsoft.UI.Xaml.Documents.Hyperlink sender, Microsoft.UI.Xaml.Documents.HyperlinkClickEventArgs args)
        {
            NavigationHelper.ToManageDB();
        }

        private void LoadTemporaryDatabase_Click(object sender, RoutedEventArgs e)
        {
            TemporaryDatabaseHelper.LoadImportedDatabase();
            loadTempDBButton.Visibility = Visibility.Collapsed;
        }
    }
}
