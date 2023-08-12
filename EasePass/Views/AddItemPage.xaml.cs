using EasePass.Models;
using Microsoft.UI;
using Microsoft.UI.Xaml.Controls;
using System.Collections.Generic;

namespace EasePass.Views
{
    public sealed partial class AddItemPage : Page
    {
        PasswordManagerItem input = null;

        public AddItemPage(List<PasswordManagerCategory> categories, PasswordManagerItem input = null)
        {
            this.InitializeComponent();

            foreach (var category in categories)
            {
                categoryCB.Items.Add(new ComboBoxItem { Content = category.CategoryName, Tag = category });
            }

            if (input == null)
                return;

            this.input = input;
            notesTB.Text = input.Notes;
            pwTB.Password = input.Password;
            emailTB.Text = input.Email;
            usernameTB.Text = input.Username;
            nameTB.Text = input.DisplayName;
            //iconFI.Glyph = input.IconId;
        }

        public (PasswordManagerItem, PasswordManagerCategory)GetValue()
        {
            if (input == null)
                input = new PasswordManagerItem();


            input.Notes = notesTB.Text;
            input.Password = pwTB.Password;
            input.Email = emailTB.Text;
            input.Username = usernameTB.Text;
            input.DisplayName = nameTB.Text;
            
            return (input, (categoryCB.SelectedItem as ComboBoxItem).Tag as PasswordManagerCategory);
        }
    }
}
