using EasePassExtensibility;
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
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace EasePass.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ImportPage : Page
    {
        ObservableCollection<PasswordItem> Items = new ObservableCollection<PasswordItem>();
        ObservableCollection<PasswordItem> items;
        bool[] Checked = null;

        public ImportPage(IPasswordImporter importer)
        {
            this.InitializeComponent();
            Task.Run(new Action(() =>
            {
                try // catch exceptions from plugin
                {
                    items = new ObservableCollection<PasswordItem>(importer.ImportPasswords());
                }
                catch
                {
                    items = null;
                }
            })).GetAwaiter().OnCompleted(new Action(() =>
            {
                if (items != null)
                {
                    if (items.Count > 0)
                    {
                        Checked = new bool[items.Count];
                        Items.Clear();
                        for (int i = 0; i < items.Count; i++)
                        {
                            Checked[i] = true;
                            Items.Add(items[i]);
                        }
                        items.Clear();
                    }
                    else
                    {
                        errorMsg.Visibility = Visibility.Visible;
                        errorMsg.Text = "No passwords available!";
                    }
                }
                else
                {
                    errorMsg.Visibility = Visibility.Visible;
                }
                progress.Visibility = Visibility.Collapsed;
            }));
        }

        public PasswordItem[] GetSelectedPasswords()
        {
            List<PasswordItem> result = new List<PasswordItem>();
            for(int i = 0; i < Items.Count; i++)
            {
                if (Checked[i]) result.Add(Items[i]);
            }
            return result.ToArray();
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            PasswordItem item = (PasswordItem)(sender as CheckBox).Tag;
            if (item != null) Checked[Items.IndexOf(item)] = true;
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            PasswordItem item = (PasswordItem)(sender as CheckBox).Tag;
            if (item != null) Checked[Items.IndexOf(item)] = false;
        }
    }
}
