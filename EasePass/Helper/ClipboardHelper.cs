using EasePass.Settings;
using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Windows.ApplicationModel.DataTransfer;

namespace EasePass.Helper
{
    internal class ClipboardHelper
    {
        static Queue<ClipboardHistoryItem> history = new Queue<ClipboardHistoryItem>();

        public static void Copy(string text, bool removeFromClipboard = false)
        {
            var package = new DataPackage();
            package.SetText(text);
            Clipboard.SetContent(package);

            //remove from the clipboard:
            if (removeFromClipboard)
                RemoveLastClipboardItem();
        }

        public static void RemoveLastClipboardItem()
        {
            // Needs to be a short delay here
            var dp = new DispatcherTimer();
            dp.Interval = new TimeSpan(0, 0, 0, 0, 1000);
            dp.Start();
            dp.Tick += async (s, e) =>
            {
                var items = (await Clipboard.GetHistoryItemsAsync()).Items;
                if (items.Count > 0)
                    history.Enqueue(items[0]);
                dp.Stop();
            };

            var dp1 = new DispatcherTimer();
            dp1.Interval = new TimeSpan(0, 0, AppSettings.GetSettingsAsInt(AppSettingsValues.clipboardClearTimeoutSec, DefaultSettingsValues.ClipboardClearTimeoutSec));
            dp1.Start();
            dp1.Tick += (s, e) =>
            {
                dp1.Stop();
                Clipboard.SetContent(null);
                try
                {
                    var item = history.Dequeue();
                    if (item != null)
                        Clipboard.DeleteItemFromHistory(item);
                }
                catch { }
            };
        }
    }
}
