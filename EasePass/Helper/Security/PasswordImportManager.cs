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
using EasePass.Helper.FileSystem;
using EasePass.Models;
using EasePassExtensibility;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EasePass.Helper.Security
{
    internal static class PasswordImportManager
    {
        public async static Task<(PasswordManagerItem[] Items, bool Override)> ManageImport(IPasswordImporter importer)
        {
            ImportPasswordsDialog dlg = new ImportPasswordsDialog();

            dlg.ShowProgressBar();

            var loadingTask = Task.Run(() =>
            {
                if (importer is IFilePickerInjectable fpi)
                    fpi.FilePicker = FilePicker;
                var items = importer.ImportPasswords();
                return items.Select(x => ToPMI(x)).ToArray();
            });

            var showDialogTask = dlg.ShowAsync(true);

            var passwordItems = await loadingTask;
            dlg.SetPagePasswords(passwordItems);

            return await showDialogTask;
        }

        private static string FilePicker(string[] extensions)
        {
            var res = Task.Run(async () => await FilePickerHelper.PickOpenFile(extensions)).Result;
            if (res.success)
                return res.path;
            return "";
        }

        private static PasswordManagerItem ToPMI(PasswordItem item)
        {
            PasswordManagerItem pmi = new PasswordManagerItem();
            pmi.DisplayName = item.DisplayName;
            pmi.Email = item.EMail;
            pmi.Username = item.UserName;
            pmi.Password = item.Password;
            pmi.Website = item.Website;
            pmi.Notes = item.Notes;
            pmi.Secret = item.TOTPSecret;
            pmi.Interval = Convert.ToString(item.TOTPInterval);
            pmi.Digits = Convert.ToString(item.TOTPDigits);
            pmi.Algorithm = item.TOTPAlgorithm.ToString();

            return pmi;
        }
    }
}
