using EasePass.Dialogs;
using EasePass.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Core;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace EasePass.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ImportPage : Page
    {
        ObservableCollection<PasswordManagerItem> Items = new ObservableCollection<PasswordManagerItem>();
        bool[] Checked = null;

        public ImportPage()
        {
            this.InitializeComponent();
        }

        public void SetMessage(ImportDialog.MsgType msg)
        {
            switch (msg)
            {
                case ImportDialog.MsgType.None:
                    progress.Visibility = Visibility.Visible;
                    break;
                case ImportDialog.MsgType.Error:
                    progress.Visibility = Visibility.Collapsed;
                    errorMsg.Text = "An error has occured!";
                    errorMsg.Visibility = Visibility.Visible;
                    break;
                case ImportDialog.MsgType.NoPasswords:
                    progress.Visibility = Visibility.Collapsed;
                    errorMsg.Text = "No passwords available!";
                    errorMsg.Visibility = Visibility.Visible;
                    break;
            }
        }

        public void SetPasswords(PasswordManagerItem[] items)
        {
            Items.Clear();
            Checked = new bool[items.Length];
            for(int i = 0; i < items.Length; i++)
            {
                Items.Add(items[i]);
                Checked[i] = true;
            }
            progress.Visibility = Visibility.Collapsed;
        }

        public PasswordManagerItem[] GetSelectedPasswords()
        {
            List<PasswordManagerItem> result = new List<PasswordManagerItem>();
            for(int i = 0; i < Items.Count; i++)
            {
                if (Checked[i]) result.Add(Items[i]);
            }
            return result.ToArray();
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            PasswordManagerItem item = (PasswordManagerItem)(sender as CheckBox).Tag;
            if (item != null) Checked[Items.IndexOf(item)] = true;
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            PasswordManagerItem item = (PasswordManagerItem)(sender as CheckBox).Tag;
            if (item != null) Checked[Items.IndexOf(item)] = false;
        }
    }
}
