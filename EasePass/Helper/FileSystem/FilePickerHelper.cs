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

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage.Pickers;

namespace EasePass.Helper.FileSystem
{
    internal class FilePickerHelper
    {
        public static async Task<(string path, bool success)> PickOpenFile(string[] extensions)
        {
            var openPicker = new FileOpenPicker();
            var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(EasePass.App.m_window);

            WinRT.Interop.InitializeWithWindow.Initialize(openPicker, hWnd);

            openPicker.ViewMode = PickerViewMode.Thumbnail;
            for (int i = 0; i < extensions.Length; i++)
            {
                openPicker.FileTypeFilter.Add(extensions[i]);
            }

            var file = await openPicker.PickSingleFileAsync();
            if (file == null)
                return (null, false);
            return (file.Path, true);
        }

        public static async Task<(string path, bool success)> PickSaveFile((string val, List<string> ext) extensions)
        {
            FileSavePicker savePicker = new FileSavePicker();

            var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(EasePass.App.m_window);
            WinRT.Interop.InitializeWithWindow.Initialize(savePicker, hWnd);

            savePicker.FileTypeChoices.Add(extensions.val, extensions.ext);

            var file = await savePicker.PickSaveFileAsync();
            if (file == null)
                return (null, false);
            return (file.Path, true);
        }

        public static async Task<(string path, bool success)> PickFolder()
        {
            FolderPicker openPicker = new FolderPicker();

            var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(EasePass.App.m_window);
            WinRT.Interop.InitializeWithWindow.Initialize(openPicker, hWnd);

            openPicker.SuggestedStartLocation = PickerLocationId.Desktop;
            openPicker.FileTypeFilter.Add("*");
            var folder = await openPicker.PickSingleFolderAsync();
            if (folder == null)
                return (null, false);
            return (folder.Path, true);
        }
    }
}
