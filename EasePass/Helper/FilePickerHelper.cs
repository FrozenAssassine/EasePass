using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage.Pickers;

namespace EasePass.Helper
{
    internal class FilePickerHelper
    {
        public static async Task<(string path,bool success)> PickOpenFile(string[] extensions)
        {
            var openPicker = new FileOpenPicker();
            var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(App.m_window);

            WinRT.Interop.InitializeWithWindow.Initialize(openPicker, hWnd);

            openPicker.ViewMode = PickerViewMode.Thumbnail;
            for(int i = 0; i< extensions.Length; i++)
            {
                openPicker.FileTypeFilter.Add(extensions[i]);
            }

            var file = await openPicker.PickSingleFileAsync();
            if(file == null)
                return (null, false);
            return (file.Path, true);
        }

        public static async Task<(string path, bool success)> PickSaveFile((string val, List<string> ext) extensions)
        {
            FileSavePicker savePicker = new FileSavePicker();

            var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(App.m_window);
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

            var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(App.m_window);
            WinRT.Interop.InitializeWithWindow.Initialize(openPicker, hWnd);

            openPicker.SuggestedStartLocation = PickerLocationId.Desktop;
            openPicker.FileTypeFilter.Add("*");
            var folder = await openPicker.PickSingleFolderAsync();
            if(folder == null)
                return (null, false);
            return (folder.Path, true);
        }
    }
}
