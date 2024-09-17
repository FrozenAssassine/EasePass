using EasePass.Dialogs;
using EasePass.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasePass.Helper
{
    internal class ExportPasswordsHelper
    {
        public async static Task Export(Database db, ObservableCollection<PasswordManagerItem> items)
        {
            var res = await FilePickerHelper.PickSaveFile(("Ease Pass database", new List<string>() { ".epdb" }));
            if (!res.success)
                return;

            var exportDialogRes = await new SelectExportPasswordsDialog().ShowAsync(items);
            if (exportDialogRes == null)
                return;

            Database export = new Database(res.path);
            export.MasterPassword = db.MasterPassword;
            export.SetNewPasswords(exportDialogRes);
            export.Save();

            InfoMessages.ExportDBSuccess();
        }

    }
}
