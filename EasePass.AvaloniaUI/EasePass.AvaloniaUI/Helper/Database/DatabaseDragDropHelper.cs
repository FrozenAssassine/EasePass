using Avalonia.Input;
using Avalonia.Platform.Storage;
using EasePass.Dialogs;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EasePass.Helper.Database
{
    internal class DatabaseDragDropHelper
    {
        public static void DragOver(DragEventArgs e)
        {
            e.DragEffects = DragDropEffects.Copy;
            e.Handled = true;
        }

        public static async Task Drop(DragEventArgs e)
        {
            if (!e.DataTransfer.Contains(DataFormat.File))
                return;

            var items = e.DataTransfer.GetItems(DataFormat.File);
            if (items == null || items.Count() == 0)
                return;

            var fileItem = items.OfType<IStorageFile>().FirstOrDefault();
            if (fileItem == null)
                return;

            if (Path.GetExtension(fileItem.Name)
                        .Equals(".epdb", StringComparison.OrdinalIgnoreCase))
            {
                await ManageDatabaseHelper.ImportIntoDatabase(fileItem.Path!.LocalPath);
            }
            else
            {
                InfoMessages.DatabaseInvalidData();
            }
        }
    }
}
