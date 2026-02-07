using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.VisualTree;
using EasePass.AvaloniaUI;
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
using EasePass.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace EasePass.Views
{
    public partial class PasswordsPage : UserControl
    {
        private PasswordsViewModel ViewModel => this.DataContext as PasswordsViewModel;
        public delegate int PasswordExists(string password);

        public PasswordsPage()
        {
            InitializeComponent();
            this.DataContext = new PasswordsViewModel();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
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
            if (pwTB == null)
                throw new Exception("pwTB must not be null");

            SortPasswordListButton = this.FindControl<DropDownButton>("SortPasswordListButton");
            if (SortPasswordListButton == null)
                throw new Exception("SortPasswordListButton must not be null");
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadControls();

            InfobarExtension.ClearAfterLogin();

            searchbox.Focus(NavigationMethod.Unspecified);

            //todo
            //var colDef = this.FindControl<ColumnDefinition>("gridSplitterLoadSize");
            //if (colDef != null)
            //{
            //     colDef.Width = new GridLength(AppSettings.GridSplitterWidth, GridUnitType.Pixel);
            //}
        }

        private void StoreGridSplitterValue()
        {
            //todo: call this function somewhere
            //todo var colDef = this.FindControl<ColumnDefinition>("gridSplitterLoadSize");
            //if (colDef != null)
            //AppSettings.GridSplitterWidth = (int)colDef.Width.Value;
        }

        private void Searchbox_PreviewKeyDown(bool isTagSearch, KeyEventArgs e)
        {
            if (e.Key == Key.Down)
            {
                e.Handled = true;
                passwordItemListView.Focus(NavigationMethod.Unspecified);
                if (passwordItemListView.SelectedIndex == -1 && passwordItemListView.Items.Count > 0)
                {
                    passwordItemListView.SelectedIndex = 0;
                }
            }
        }


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
                        ViewModel?.AddItemCommand?.Execute(null);
                        break;
                    case Key.E:
                        ViewModel?.EditItemCommand?.Execute(ViewModel.SelectedItem);
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
                        ViewModel?.LogoutCommand.Execute(null);
                        break;
                    default:
                        return;
                }
            }

            switch (e.Key)
            {
                case Key.F1:
                    ViewModel?.SettingsCommand.Execute(null);
                    break;
                case Key.F2:
                    ViewModel?.EditItemCommand.Execute(ViewModel.SelectedItem);
                    break;
                case Key.Escape:
                    passwordItemListView.SelectedItem = null;
                    if (ViewModel != null) ViewModel.SelectedItem = null;
                    break;
            }
        }



        private void Searchbox_TextChanged(object sender, bool isUserTextChange, string text)
        {
            if (ViewModel != null)
                ViewModel.SearchText = text;
        }

        private void Searchbox_SuggestionChosen(object sender, SelectionChangedEventArgs e)
        {
            if (ViewModel != null && e.AddedItems.Count > 0)
            {
                ViewModel.SearchBoxSelectedItem = e.AddedItems[0];
            }
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
            ViewModel?.NavigateToManageDBCommand.Execute(null);
        }
    }
}
