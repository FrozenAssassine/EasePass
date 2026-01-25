using EasePass.Controls;
using EasePass.Core.Database;
using EasePass.Models;
using Microsoft.UI.Xaml.Controls;
using System.Collections.ObjectModel;

namespace EasePass.Helper
{
    internal class SearchPasswordsManager
    {
        public TagSearchManager tagSearchManager { get; } = new TagSearchManager();

        public ObservableCollection<PasswordManagerItem> SearchPasswords(
            SearchPasswordsBox searchbox, 
            AutoSuggestBox sender, 
            DatabaseItem loadedDB, 
            bool isUserTextChange, 
            string text)
        {
            if (!isUserTextChange)
            {
                if (text.StartsWith("/"))
                {
                    var tagSearchRes = loadedDB.FindItemsByTag(searchbox.Text.Replace("/", ""));
                    searchbox.InfoLabel = tagSearchRes.Count.ToString();
                    return tagSearchRes;
                }
                return null;
            }

            if (text == null || text.Length == 0)
            {
                sender.ItemsSource = null;
                searchbox.InfoLabel = loadedDB.Items.Count.ToString();
                return loadedDB.Items;
            }

            if (text.StartsWith("/"))
            {
                if (tagSearchManager.UniqueTagList == null)
                    return null;

                var searchText = text.Replace("/", "");
                var suitableItems = tagSearchManager.Filter(searchText, "/");
                if (suitableItems.Count == 0)
                {
                    suitableItems.Add("No results found");
                }

                if (searchbox.SelectedItem == null)
                {
                    var tagSearchRes = loadedDB.FindItemsByTag(searchText);
                    searchbox.InfoLabel = tagSearchRes.Count.ToString();
                    sender.ItemsSource = suitableItems;
                    return tagSearchRes;
                }

                sender.ItemsSource = suitableItems;
                return null;
            }

            sender.ItemsSource = null;

            var search_res = loadedDB.FindItemsByName(text);
            searchbox.InfoLabel = search_res.Count.ToString();
            return search_res;
        }


    }
}
