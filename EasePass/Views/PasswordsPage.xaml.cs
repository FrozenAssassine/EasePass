using EasePass.Dialogs;
using EasePass.Helper;
using EasePass.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Security;
using Windows.ApplicationModel.DataTransfer;
using Windows.System;

namespace EasePass.Views
{
    public sealed partial class PasswordsPage : Page
    {
        private ObservableCollection<PasswordManagerItem> PasswordItems = new ObservableCollection<PasswordManagerItem>();
        //{
        //    new PasswordManagerItem("github", "\xE731", "1231241234", "MyUserName", "MyEmail@Emai.de", "This are notes\nMore notes"),
        //    new PasswordManagerItem("otherhub", "1231241234", "MyUserName2", "MyEmail@Email2.de", "This are notes\nMore notes\and More"),
        //    new PasswordManagerItem("thinkhub", "1231241234", "MyUserName5", "MyEmail@Email5.de", "This are notes\nMore notes"),
        //    new PasswordManagerItem("Amazon", "\xE7BA", "asldkasldök1", "MyUserNam8e", "MyEmail@Emai12.de", "This are notes\nMore notes"),
        //    new PasswordManagerItem("Paypal", "asldkasldök5", "MyUserName9", "MyEmail@Emai5.de", "This are notes\nMore notes\and More"),
        //    new PasswordManagerItem("Baguette Bank", "asldkasldök100", "MyUserName6", "MyEmail@Email5.de", "This are notes\nMore notes"),
        //};

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
            pwTB.Password = item.Password;
            emailTB.Text = item.Email;
            usernameTB.Text = item.Username;
            itemnameTB.Text = item.DisplayName;

            passwordShowArea.Visibility = Visibility.Visible;
        }


        private void passwordItemListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (passwordItemListView.SelectedItem == null)
                return;

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
        private void ShowPassword_Changed(object sender, RoutedEventArgs e)
        {
            if(sender is CheckBox cb)
                pwTB.PasswordRevealMode = cb.IsChecked ?? false ? PasswordRevealMode.Visible : PasswordRevealMode.Hidden;
        }
        private void pwTB_PreviewKeyDown(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            e.Handled = true;
            if (e.Key == VirtualKey.A && KeyHelper.IsKeyPressed(VirtualKey.Control))
                pwTB.SelectAll();
            else if(e.Key == VirtualKey.C && KeyHelper.IsKeyPressed(VirtualKey.Control))
                ClipboardHelper.Copy(pwTB.Password);
        }
        private void TB_SelectAl_Click(object sender, RoutedEventArgs e)
        {
            if(rightClickedItem is TextBox tb)
                tb.SelectAll();
            else if(rightClickedItem is PasswordBox pb) 
                pb.SelectAll();
        }
        private void TB_Copy_Click(object sender, RoutedEventArgs e)
        {
            if (rightClickedItem is TextBox tb)
                ClipboardHelper.Copy(tb.Text);
            else if (rightClickedItem is PasswordBox pb)
                ClipboardHelper.Copy(pb.Password);
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            App.m_frame.Navigate(typeof(SettingsPage), PasswordItems);
        }
        private void AboutPage_Click(object sender, RoutedEventArgs e)
        {
            App.m_frame.Navigate(typeof(AboutPage));
        }

        private void tb_RightTapped(object sender, Microsoft.UI.Xaml.Input.RightTappedRoutedEventArgs e)
        {
            rightClickedItem = sender as FrameworkElement;
            textboxFlyout.ShowAt(rightClickedItem);
        }
    }
}
