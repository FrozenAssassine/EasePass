/*
MIT License

Copyright (c) 2023 Julius Kirsch

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.
*/

using Avalonia;
using Avalonia.Threading;
using EasePass.AvaloniaUI.Views;
using EasePass.Settings;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EasePass.Helper
{
    internal class ClipboardHelper
    {
        //static Queue<ClipboardHistoryItem> history = new Queue<ClipboardHistoryItem>();

        public static async Task CopyAsync(string text, bool removeFromClipboard = false)
        {
            if (string.IsNullOrEmpty(text))
                return;

            var clipboard = MainWindow.current.Clipboard;
            if (clipboard != null)
            {
                await clipboard.SetTextAsync(text);
            }

            if (removeFromClipboard)
            {
                //ScheduleRemoveAsync(text, AppSettings.ClipboardClearTimeoutSec * 1000);
            }
        }

        //todo: implement clipboard history removal
        /* public static void RemoveLastClipboardItem(string text)
        {
            // Needs to be a short delay here
            var dp = new DispatcherTimer();
            dp.Interval = new TimeSpan(0, 0, 0, 0, 1000);
            dp.Start();
            dp.Tick += async (s, e) =>
            {
                var items = (await Clipboard.GetHistoryItemsAsync()).Items;
                if (items.Count > 0)
                {
                    for(int i = 0; i < items.Count; i++)
                    {
                        try
                        {
                            if (await items[i].Content.GetTextAsync() == text)
                            {
                                history.Enqueue(items[i]);
                                break;
                            }
                        }
                        catch { }
                    }
                }
                dp.Stop();
            };

            var dp1 = new DispatcherTimer();
            
            dp1.Interval = new TimeSpan(0, 0, AppSettings.ClipboardClearTimeoutSec);
            dp1.Start();
            dp1.Tick += async (s, e) =>
            {
                dp1.Stop();
                var content = Clipboard.GetContent();
                if (content == null || !content.Contains(StandardDataFormats.Text))
                {
                    return;
                }
                if (await Clipboard.GetContent().GetTextAsync() == text)
                {
                    Clipboard.SetContent(null);
                }
                var items = (await Clipboard.GetHistoryItemsAsync()).Items;
                if (items.Count > 0)
                {
                    for (int i = 0; i < items.Count; i++)
                    {
                        try
                        {
                            if (await items[i].Content.GetTextAsync() == text)
                            {
                                history.Enqueue(items[i]);
                                break;
                            }
                        }
                        catch { }
                    }
                }
            };
        }*/
    }
}
