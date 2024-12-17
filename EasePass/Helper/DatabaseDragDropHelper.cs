using EasePass.Dialogs;
using Microsoft.UI.Xaml;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;

namespace EasePass.Helper
{
    internal class DatabaseDragDropHelper
    {
        public static void DragOver(DragEventArgs e)
        {
            e.AcceptedOperation = DataPackageOperation.Copy;
        }

        public static async Task Drop(DragEventArgs e)
        {
            //only accept files with .epdb format:
            if (!e.DataView.Contains(StandardDataFormats.StorageItems))
                return;

            var storageItems = await e.DataView.GetStorageItemsAsync();
            if (storageItems == null || storageItems.Count == 0)
                return;

            Debug.WriteLine(storageItems[0].Path + "::" + Path.GetExtension(storageItems[0].Path));
            if (Path.GetExtension(storageItems[0].Path).Equals(".epdb", StringComparison.OrdinalIgnoreCase))
            {
                var files = await e.DataView.GetStorageItemsAsync();
                await ManageDatabaseHelper.ImportIntoDatabase(files[0].Path);
            }
            else
                InfoMessages.DatabaseInvalidData();
        }
    }
}
