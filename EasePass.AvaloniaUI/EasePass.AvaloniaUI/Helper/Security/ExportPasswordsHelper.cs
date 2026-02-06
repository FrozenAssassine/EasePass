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

using EasePass.Core.Database;
using EasePass.Dialogs;
using EasePass.Helper.FileSystem;
using EasePass.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Security;
using System.Threading.Tasks;

namespace EasePass.Helper.Security;
internal class ExportPasswordsHelper
{
    public async static Task Export(DatabaseItem db, ObservableCollection<PasswordManagerItem> items, SecureString newPassword = null, bool showSelectPasswordDialog = true)
    {
        var res = await FilePickerHelper.PickSaveFile(("Ease Pass database", new List<string>() { ".epdb" }));
        if (!res.success)
            return;

        DatabaseItem export = new DatabaseItem(new NativeDatabaseSource(res.path));
        export.MasterPassword = newPassword ?? db.MasterPassword;
        export.SecondFactor = db.SecondFactor;
        export.Settings = db.Settings;

        if (showSelectPasswordDialog)
        {
            PasswordManagerItem[] exportItems = await new SelectExportPasswordsDialog().ShowAsync(items);
            if (exportItems == null)
                return;

            export.SetNewPasswords(exportItems);
        }
        else
        {
            export.SetNewPasswords(items);
        }

        if(await export.ForceSaveAsync())
            InfoMessages.ExportDBSuccess();
        else
            InfoMessages.ExportDBFailed();
    }
}
