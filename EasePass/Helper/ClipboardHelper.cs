using EasePass.Settings;
using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Windows.ApplicationModel.DataTransfer;

namespace EasePass.Helper
{
    internal class ClipboardHelper
    {
        private static ClipboardHistoryItem ClipboardHistoryItem = null;

        public static void Copy(string text, bool removeFromClipboard = false)
        {
            var package = new DataPackage();
            package.SetText(text);
            Clipboard.SetContent(package);

            //remove from the clipboard:
            if (removeFromClipboard)
                RemoveLastClipboardItem();
        }

        public static async void RemoveLastClipboardItem()
        {
            var items = (await Clipboard.GetHistoryItemsAsync()).Items;

            if (items.Count > 0)
                ClipboardHistoryItem = items[0];
            var dp = new DispatcherTimer();
            dp.Interval += new TimeSpan(0, 0, AppSettings.GetSettingsAsInt(AppSettingsValues.clipboardClearTimeoutSec, DefaultSettingsValues.ClipboardClearTimeoutSec));
            dp.Start();
            dp.Tick += (s, e) =>
            {
                if (ClipboardHistoryItem != null)
                    Clipboard.DeleteItemFromHistory(ClipboardHistoryItem);
                Clipboard.SetContent(null);
                ClipboardHistoryItem = null;
                dp.Stop();
            };
        }
    }
}
