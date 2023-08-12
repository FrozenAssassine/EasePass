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
        public static MenuFlyout passwordItemFlyout;
        public static NavigationViewItem rightClickedItem;

        private static NavigationViewItem CreateItem(PasswordManagerItem item)
        {
            var navItem = new NavigationViewItem
            {
                Content = item.DisplayName,
                Tag = item,
            };
            navItem.RightTapped += PasswordManagerItem_RightTapped;
            if (item.IconId != null)
                navItem.Icon = new FontIcon { FontFamily = new Microsoft.UI.Xaml.Media.FontFamily("Segoe MDL2 Assets"), Glyph = item.IconId };

            return navItem;
        }

        private static void PasswordManagerItem_RightTapped(object sender, Microsoft.UI.Xaml.Input.RightTappedRoutedEventArgs e)
        {
            rightClickedItem = sender as NavigationViewItem;
            passwordItemFlyout.ShowAt(rightClickedItem);
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
        
        public static void PopulateNaivgationView(NavigationView navigationView, List<PasswordManagerCategory> categories)
        {
            foreach(var categoryItem in categories)
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

        public static void DeleteItem(NavigationView navigationView, List<PasswordManagerCategory> categories, PasswordManagerItem item)
        {
            var category = categories.Find(x=>x.Items.Contains(item));
            int categoryIndex = categories.IndexOf(category);
            int itemIndex = category.Items.IndexOf(item);

            (navigationView.MenuItems[categoryIndex] as NavigationViewItem).MenuItems.RemoveAt(itemIndex);
            category.Items.Remove(item);
        }
    
        public static void AddItem(NavigationView navigationView, List<PasswordManagerCategory> categories, PasswordManagerItem item, PasswordManagerCategory category)
        {
            var categoryItem = categories[categories.IndexOf(category)];
            categoryItem.Items.Add(item);

            (navigationView.MenuItems[categories.IndexOf(categoryItem)] as NavigationViewItem).MenuItems.Add(CreateItem(item));
        }

        public static void UpdateItem(NavigationView navigationView, List<PasswordManagerCategory> categories, PasswordManagerItem item, PasswordManagerCategory newCategory)
        {
            var category = categories.Find(x => x.Items.Contains(item));
            int categoryIndex = categories.IndexOf(category);
            int itemIndex = category.Items.IndexOf(item);

            var navItem = (navigationView.MenuItems[categoryIndex] as NavigationViewItem).MenuItems[itemIndex] as NavigationViewItem;
            navItem.Content = item.DisplayName;
            navItem.Tag = item;
            if (item.IconId != null)
                navItem.Icon = new FontIcon { FontFamily = new Microsoft.UI.Xaml.Media.FontFamily("Segoe MDL2 Assets"), Glyph = item.IconId };
        
            if(newCategory != null && category != newCategory)
            {
                UpdateCategory(navigationView, categories, item, newCategory);
            }
        }

        public static void UpdateCategory(NavigationView navigationView, List<PasswordManagerCategory> categories, PasswordManagerItem item, PasswordManagerCategory newCategory)
        {
            //Update list:
            var currentCategory = categories.Find(x => x.Items.Contains(item));
            int oldCategoryIndex = categories.IndexOf(currentCategory);
            int oldItemIndex = currentCategory.Items.IndexOf(item);
            currentCategory.Items.Remove(item);
            newCategory.Items.Add(item);

            //Update navigationView:

            int newCategoryIndex = categories.IndexOf(newCategory);
            (navigationView.MenuItems[oldCategoryIndex] as NavigationViewItem).MenuItems.RemoveAt(oldItemIndex);
            (navigationView.MenuItems[newCategoryIndex] as NavigationViewItem).MenuItems.Add(CreateItem(item));
            
        }
    }
}
