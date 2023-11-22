using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Windows.ApplicationModel.DataTransfer;

namespace EasePass.Helper
{
    internal class ClipboardHelper
    {
        private static Stack<ClipboardHistoryItem> ClipboardHistoryItems = new Stack<ClipboardHistoryItem>();

        public static void Copy(string text, bool removeFromClipboard = false)
        {
            var package = new DataPackage();
            package.SetText(text);
            Clipboard.SetContent(package);

            //remove from the clipboard:
            if(removeFromClipboard)
                RemoveLastClipboardItem();
        }

        public static async void RemoveLastClipboardItem()
        {
            var items = (await Clipboard.GetHistoryItemsAsync()).Items;
            if (items.Count == 0)
                return;

            ClipboardHistoryItems.Push(items[0]);
            var dp = new DispatcherTimer();
            dp.Interval += new TimeSpan(0, 0, 10);
            dp.Start();
            dp.Tick += async (s, e) =>
            {
                var removeItem = ClipboardHistoryItems.Pop();
                Clipboard.DeleteItemFromHistory(removeItem);
                dp.Stop();
            };
        }
    }
}
