using EasePass.Models;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Services.Maps;

namespace EasePass.Helper
{
    internal class PasswordItemsManager
    {
        private static NavigationViewItem CreateItem(PasswordManagerItem item)
        {
            var navItem = new NavigationViewItem
            {
                Content = item.DisplayName,
                Tag = item,
            };
            if (item.IconId != null)
                navItem.Icon = new FontIcon { FontFamily = new Microsoft.UI.Xaml.Media.FontFamily("Segoe MDL2 Assets"), Glyph = item.IconId };

            return navItem;
        }
        private static NavigationViewItem CreateCategory(PasswordManagerCategory category)
        {
            var navItem = new NavigationViewItem { Content = category.CategoryName };

            if(category.IconId != null)
                navItem.Icon = new FontIcon { FontFamily = new Microsoft.UI.Xaml.Media.FontFamily("Segoe MDL2 Assets"), Glyph = category.IconId };

            foreach (var pwItem in category.Items)
            {
                navItem.MenuItems.Add(CreateItem(pwItem));
            }

            return navItem;
        }
        
        public static void PopulateNaivgationView(NavigationView navigationView, List<PasswordManagerCategory> items)
        {
            foreach(var categoryItem in items)
            {
                navigationView.MenuItems.Add(CreateCategory(categoryItem));
            }
        }

        public static List<PasswordManagerItem> FindItemsByName(List<PasswordManagerCategory> categories, string name)
        {
            List<PasswordManagerItem> foundItems = new List<PasswordManagerItem>();
            foreach (var category in categories)
            {
                foundItems.AddRange(category.Items.Where(item => item.DisplayName == name));
            }
            return foundItems;
        }

    }
}
