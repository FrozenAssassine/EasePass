using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

            savePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            savePicker.FileTypeChoices.Add(extensions.val, extensions.ext);
            
            var file = await savePicker.PickSaveFileAsync();
            if (file == null)
                return (null, false);
            return (file.Path, true);
        }
    }
}
