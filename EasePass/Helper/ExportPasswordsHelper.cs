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

using EasePass.Dialogs;
using EasePass.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
