using Avalonia.Input;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EasePass.AvaloniaUI;
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
using EasePass.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace EasePass.ViewModels
{
    public partial class PasswordsViewModel : ViewModelBase, IDisposable
    {
        private SearchPasswordsManager searchPwManager = new SearchPasswordsManager();
        private DispatcherTimer totpTimer;

        [ObservableProperty]
        private ObservableCollection<PasswordManagerItem> _passwordItems;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(EditItemCommand))]
        [NotifyCanExecuteChangedFor(nameof(DeleteItemCommand))]
        private PasswordManagerItem _selectedItem;

        [ObservableProperty]
        private string _searchText = "";
        
        [ObservableProperty]
        private object _searchBoxSelectedItem;

        [ObservableProperty]
        private string _totpToken;

        [ObservableProperty]
        private string _searchInfoLabel;

        [ObservableProperty]
        private bool _isOobeVisible;

        [ObservableProperty]
        private DatabaseItem _loadedDB;

        [ObservableProperty]
        private bool _showTempDBButton;
        
        [ObservableProperty]
        private bool _passwordShowAreaVisible = false;

        public PasswordsViewModel()
        {
            if (Database.LoadedInstance != null)
            {
                LoadedDB = Database.LoadedInstance;
                PasswordItems = Database.LoadedInstance.Items;
                ShowTempDBButton = Core.Database.Database.LoadedInstance.IsTemporaryDatabase;
                
                LoadedDB.PropertyChanged += LoadedDB_PropertyChanged;
            }

            searchPwManager.tagSearchManager.UniqueTagList?.Clear();
            UpdateSearchInfo();
            UpdateOOBEGrid();

            AppVersionHelper.CheckNewVersion(); // From Page_Loaded
        }

        private void LoadedDB_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsReadonlyDatabase")
            {
                OnPropertyChanged(nameof(LoadedDB));
            }
        }


        partial void OnSearchTextChanged(string value) => PerformSearch();
        
        partial void OnSearchBoxSelectedItemChanged(object value) => PerformSearch();

        private void PerformSearch()
        {
            if (LoadedDB == null) return;
            string text = SearchText;

            if (string.IsNullOrEmpty(text))
            {
                PasswordItems = LoadedDB.Items;
                UpdateSearchInfo();
                return;
            }
            
            if (text.StartsWith("/"))
            {
                if (searchPwManager.tagSearchManager.UniqueTagList == null)
                {
                    searchPwManager.tagSearchManager.UpdateTagList(LoadedDB);
                }

                var cleanText = text.Replace("/", "");
                if (SearchBoxSelectedItem == null)
                {
                    var tagSearchRes = LoadedDB.FindItemsByTag(cleanText);
                    PasswordItems = tagSearchRes;
                    SearchInfoLabel = tagSearchRes.Count.ToString();
                    return;
                }
            }

            var searchRes = LoadedDB.FindItemsByName(text);
            PasswordItems = searchRes;
            SearchInfoLabel = searchRes.Count.ToString();
            
            if (searchRes.Count == 1)
            {
                SelectedItem = searchRes[0];
            }
        }

        partial void OnSelectedItemChanged(PasswordManagerItem value)
        {
            UpdateOOBEGrid();
            PasswordShowAreaVisible = value != null;

            if (value != null)
            {
                value.Clicks.Add(DateTime.Now.ToString("d").Replace("/", "."));
                Update2FATimer();
            }
            else
            {
                Stop2FATimer();
            }
        }

        private void UpdateSearchInfo()
        {
            if (PasswordItems != null)
                SearchInfoLabel = PasswordItems.Count.ToString();
        }

        private void UpdateOOBEGrid()
        {
             if (LoadedDB != null)
                IsOobeVisible = LoadedDB.Items.Count == 0;
        }

        public void Reload()
        {
            // Just refresh reference or force update
            if (LoadedDB != null)
            {
                var currentItems = PasswordItems;
                PasswordItems = null;
                PasswordItems = currentItems;
            }
        }
        
        [RelayCommand]
        private async Task DeleteItem(PasswordManagerItem item)
        {
            if (item == null) return;

            if (await new DeleteConfirmationDialog().ShowAsync(item))
            {
                LoadedDB.DeleteItem(item);
                
                UpdateAfterDelete();
                LoadedDB.ScheduleSave();
            }
        }
        
        [RelayCommand]
        private async Task DeleteItems(IList<object> items)
        {
             if (items == null || items.Count == 0) return;
             
             var typedItems = items.OfType<PasswordManagerItem>().ToArray();
             if (typedItems.Length == 0) return;

             if (typedItems.Length == 1)
             {
                 await DeleteItem(typedItems[0]);
                 return;
             }

             if (await new DeleteConfirmationDialog().ShowAsync(typedItems))
             {
                 foreach(var item in typedItems)
                 {
                     LoadedDB.DeleteItem(item);
                 }
                 UpdateAfterDelete();
                 LoadedDB.ScheduleSave();
            }
        }

        private void UpdateAfterDelete()
        {
             UpdateOOBEGrid();
             if (!string.IsNullOrEmpty(SearchText))
             {
                 OnSearchTextChanged(SearchText);
             }
        }

        [RelayCommand]
        private async Task EditItem(PasswordManagerItem item)
        {
            if (item == null) return;

            PasswordManagerItem editItem = await new EditItemDialog().ShowAsync(LoadedDB.GetPasswordOccurence, item);
            if (editItem != null)
            {
                LoadedDB.ScheduleSave();

                OnSearchTextChanged(SearchText);
                SelectedItem = editItem;
            }
        }

        [RelayCommand]
        private async Task AddItem()
        {
            PasswordManagerItem newItem = await new AddItemDialog().ShowAsync(LoadedDB.GetPasswordOccurence);
            if (newItem != null)
            {
                LoadedDB.AddItem(newItem);
                OnSearchTextChanged(SearchText);
                SelectedItem = newItem;
                LoadedDB.ScheduleSave();
            }
        }

        [RelayCommand]
        private async Task Add2FA(PasswordManagerItem item)
        {
            if (item == null) return;
            if (!await new Add2FADialog().ShowAsync(item)) return;

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
            LoadedDB.ScheduleSave();
        }

        [RelayCommand]
        private async Task GeneratePassword()
        {
            await new GenPasswordDialog().ShowAsync();
        }

        [RelayCommand]
        private async Task CopyTOTP(PasswordManagerItem item)
        {
            if (item?.Secret == null) return;
            var generated = await TOTPTokenUpdater.generateCurrent(item);
            await ClipboardHelper.CopyAsync(generated.Replace(" ", ""));
        }

        [RelayCommand]
        private void Settings() => NavigationHelper.ToSettings();

        [RelayCommand]
        private void About() => NavigationHelper.ToAboutPage();
        
        [RelayCommand]
        private void NavigateToManageDB() => NavigationHelper.ToManageDB();

        [RelayCommand]
        private async Task Logout()
        {
            await App.MainVM.SaveDatabaseAsync();
            LogoutHelper.Logout();
        }

        [RelayCommand]
        private void LoadTempDB()
        {
            TemporaryDatabaseHelper.LoadImportedDatabase();
            ShowTempDBButton = false;
        }

        [RelayCommand]
        private async Task Sort(string sortType)
        {
            Comparison<PasswordManagerItem> comparison = sortType switch {
                "DisplayName" => SortingHelper.ByDisplayName,
                "Username" => SortingHelper.ByUsername,
                "Notes" => SortingHelper.ByNotes,
                "Website" => SortingHelper.ByWebsite,
                "PopularAllTime" => SortingHelper.ByPopularAllTime,
                "PopularLast30Days" => SortingHelper.ByPopularLast30Days,
                "PasswordStrength" => SortingHelper.ByPasswordStrength,
                "FirstTag" => SortingHelper.ByFirstTag,
                _ => SortingHelper.ByDisplayName
            };

            LoadedDB.Items.Sort(comparison);
            Reload();
            LoadedDB.ScheduleSave();
        }

        [RelayCommand]
        private async Task SwitchSortOrder()
        {
            LoadedDB.SetNewPasswords(LoadedDB.Items.ReverseSelf());
            Reload();
            LoadedDB.ScheduleSave();
            OnSearchTextChanged(SearchText);
        }

        [RelayCommand]
        private async void CopyUsername(PasswordManagerItem item) => await ClipboardHelper.CopyAsync(item?.Username);

        [RelayCommand]
        private async void CopyEmail(PasswordManagerItem item) => await ClipboardHelper.CopyAsync(item?.Email);

        [RelayCommand]
        private async void CopyPassword(PasswordManagerItem item) => await ClipboardHelper.CopyAsync(item?.Password, true);

        [RelayCommand]
        private async Task ExportSelected(IList<object> items)
        {
            if (items == null || items.Count == 0) return;
            var typedItems = new ObservableCollection<PasswordManagerItem>(items.OfType<PasswordManagerItem>());
            await ExportPasswordsHelper.Export(LoadedDB, typedItems);
        }

         [RelayCommand]
        private async Task ExportDiffPassword(IList<object> items)
        {
            var dialog = await new EnterPasswordDialog().ShowAsync();
            if (dialog.Password == null) return;

            if (items == null || items.Count == 0) return;
            var typedItems = new ObservableCollection<PasswordManagerItem>(items.OfType<PasswordManagerItem>());
            
            await ExportPasswordsHelper.Export(LoadedDB, typedItems, dialog.Password, false);
        }

        private void Update2FATimer()
        {
            Stop2FATimer();

            if (SelectedItem != null && !string.IsNullOrEmpty(SelectedItem.Secret))
            {
                totpTimer = new DispatcherTimer();
                totpTimer.Interval = TimeSpan.FromSeconds(1);
                totpTimer.Tick += async (s, e) => 
                {
                    if (SelectedItem != null && !string.IsNullOrEmpty(SelectedItem.Secret))
                        TotpToken = await TOTPTokenUpdater.generateCurrent(SelectedItem);
                };
                totpTimer.Start();

                Task.Run(async () => {
                     var token = await TOTPTokenUpdater.generateCurrent(SelectedItem);
                     Dispatcher.UIThread.InvokeAsync(() => TotpToken = token);
                });
            }
        }

        private void Stop2FATimer()
        {
            totpTimer?.Stop();
            totpTimer = null;
            TotpToken = "";
        }
        
        public void Dispose()
        {
            Stop2FATimer();
        }
    }
}
